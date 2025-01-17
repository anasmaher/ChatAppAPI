﻿using Application.DTOs.RelationshipDTOs;
using Application.DTOs.ResultsDTOs;
using Application.Interfaces.ReposInterfaces;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Utilities;

namespace Application.Services
{
    public class UserRelationshipService : IUserRelationshipService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly INotificationService notificationService;
        private readonly UserManager<AppUser> userManager;

        public UserRelationshipService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService
            , UserManager<AppUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.notificationService = notificationService;
            this.userManager = userManager;
        }

        public async Task<ServiceResult> BlockUserAsync(string userId, string blockedUserId)
        {
            var (orderedUseId1, orderedUserId2) = UserHelpers.GetOrderedUserIds(userId, blockedUserId);

            var relationship = await unitOfWork.UserRelationshipRepo.GetFriendshipAsync(userId, blockedUserId);

            if (relationship is null)
            {
                relationship = new UserRelationship
                {
                    User1Id = orderedUseId1,
                    User2Id = orderedUserId2,
                    Status = RelationshipStatusEnum.Blocked,
                    CreatedAt = DateTime.UtcNow,
                    ActionUserId = userId
                };

                await unitOfWork.UserRelationshipRepo.AddAsync(relationship);
            }
            else
            {
                relationship.Status = RelationshipStatusEnum.Blocked;
                relationship.ActionUserId = userId;

                unitOfWork.UserRelationshipRepo.Update(relationship);
            }

            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: "User was blocked");
        }

        public async Task<ServiceResult> GetFriendRequestsAsync(string userId, int pageNubmer, int pageSize)
        {
            var requests = await unitOfWork.UserRelationshipRepo.GetFriendRequestsAsync(userId, pageNubmer, pageSize);

            var requestsDTO = mapper.Map<IEnumerable<FriendRequestDTO>>(requests);

            return new ServiceResult(true, data: requestsDTO);
        }

        public async Task<ServiceResult> GetFriendsAsync(string userId, int pageNubmer, int pageSize)
        {
            var friends = await unitOfWork.UserRelationshipRepo.GetFriendsAsync(userId, pageNubmer, pageSize);

            var friendsDTO = mapper.Map<List<RelationMemberDTO>>(friends, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return new ServiceResult(true, data: friendsDTO);
        }

        public async Task<ServiceResult> RemoveFriendAsync(string userId, string friendUserId)
        {
            var (orderedUserId1, orderedUserId2) = UserHelpers.GetOrderedUserIds(userId, friendUserId);

            var relationship = await unitOfWork.UserRelationshipRepo.GetFriendshipAsync(orderedUserId1, orderedUserId2);

            if (relationship is null || relationship.Status is not RelationshipStatusEnum.Accepted)
                return new ServiceResult(false, ["Users are not friends"]);

            await unitOfWork.UserRelationshipRepo.RemoveAsync(r => r.Id == relationship.Id);
            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: "Friend was removed");
        }

        public async Task<ServiceResult> RespondToFriendRequestAsync(int requestId, string responderUserId, string action)
        {
            var relationship = await unitOfWork.UserRelationshipRepo.GetSingleAsync(r => r.Id == requestId);

            if (relationship is null || relationship.Status is not RelationshipStatusEnum.Pending)
                return new ServiceResult(false, ["Request does not exist"]);

            if (relationship.User1Id != responderUserId && relationship.User2Id != responderUserId)
                return new ServiceResult(false, ["Not authorized to respond to this friend request"]);

            if (action.Equals("Accept", StringComparison.OrdinalIgnoreCase))
            {
                relationship.Status = RelationshipStatusEnum.Accepted;
                relationship.ActionUserId = responderUserId;

                unitOfWork.UserRelationshipRepo.Update(relationship);

                var senderUserId = relationship.User1Id == responderUserId ? relationship.User2Id : relationship.User1Id;
                var notification = new Notification
                {
                    UserId = senderUserId,
                    SenderUserId = responderUserId,
                    Type = NotificationEnum.FriendRequestAccepted,
                    Message = "Your friend request was accepted.",
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false
                };
                await unitOfWork.NotificationRepo.AddAsync(notification);
                await unitOfWork.CommitAsync();

                try
                {
                    await notificationService.SendFriendRequestAcceptedNotificationAsync(senderUserId, responderUserId);
                }
                catch (Exception ex)
                {
                    //logger
                }

                return new ServiceResult(true, data: "Request was accepted");
            }
            else if (action.Equals("Decline", StringComparison.OrdinalIgnoreCase))
            {
                relationship.Status = RelationshipStatusEnum.Declined;
                relationship.ActionUserId = responderUserId;

                await unitOfWork.UserRelationshipRepo.RemoveAsync(r => r.Id == relationship.Id);
                await unitOfWork.CommitAsync();

                return new ServiceResult(true, data: "Request was Declined");
            }
            else
                return new ServiceResult(false, ["Action must be 'Accept' or 'Decline'"]);
        }

        public async Task<ServiceResult> SendFriendRequestAsync(string senderUserId, string recipientUserId)
        {
            if (senderUserId == recipientUserId)
                return new ServiceResult(false, ["Cannot send a friend request to yourself"]);

            var (orderedUserId1, orderedUserId2) = UserHelpers.GetOrderedUserIds(senderUserId, recipientUserId);

            var existingFriendship = await unitOfWork.UserRelationshipRepo.GetFriendshipAsync(orderedUserId1, orderedUserId2);

            if (existingFriendship is not null)
            {
                if (existingFriendship.Status is RelationshipStatusEnum.Accepted)
                    return new ServiceResult(false, ["You are already friends with this user"]);

                if (existingFriendship.Status is RelationshipStatusEnum.Pending)
                    return new ServiceResult(false, ["You already Sent a request for this user"]);

                if (existingFriendship.Status is RelationshipStatusEnum.Blocked)
                    return new ServiceResult(false, ["You cannot send a request to a blocked user"]);
            }

            var friendship = new UserRelationship
            {
                User1Id = orderedUserId1,
                User2Id = orderedUserId2,
                Status = RelationshipStatusEnum.Pending,
                CreatedAt = DateTime.UtcNow,
                ActionUserId = senderUserId
            };

            await unitOfWork.UserRelationshipRepo.AddAsync(friendship);

            var notification = new Notification
            {
                UserId = recipientUserId,
                SenderUserId = senderUserId,
                Type = NotificationEnum.FriendRequestReceived,
                Message = "You have received a friend request.",
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            };
            await unitOfWork.NotificationRepo.AddAsync(notification);
            await unitOfWork.CommitAsync();

            try
            {
                await notificationService.SendFriendRequestNotificationAsync(recipientUserId, senderUserId);
            }
            catch (Exception ex)
            {
                //logger
            }

            return new ServiceResult(true, data: "Request was sent");
        }

        public async Task<ServiceResult> UnblockUserAsync(string userId, string blockedUserId)
        {
            var (orderedUserId1, orderedUserId2) = UserHelpers.GetOrderedUserIds(userId, blockedUserId);

            var relationship = await unitOfWork.UserRelationshipRepo.GetFriendshipAsync(orderedUserId1, orderedUserId2);

            if (relationship is null || relationship.Status is not RelationshipStatusEnum.Blocked)
                return new ServiceResult(false, ["User is not blocked"]);

            await unitOfWork.UserRelationshipRepo.RemoveAsync(r => r.Id == relationship.Id);
            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: "User was Unblocked");
        }

        public async Task<ServiceResult> GetBlockedUsers(string userId, int pageNubmer, int pageSize)
        {
            var blockedUsers = await unitOfWork.UserRelationshipRepo.GetBlockedUsersAsync(userId, pageNubmer, pageSize);

            var blockedUsersDTO = mapper.Map<List<RelationMemberDTO>>(blockedUsers, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return new ServiceResult(true, data: blockedUsersDTO);
        }
    }
}
