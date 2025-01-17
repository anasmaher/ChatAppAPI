using Application.DTOs.RelationshipDTOs;
using Application.DTOs.ResultsDTOs;
using Application.Interfaces.ReposInterfaces;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Utilities;

namespace Application.Services
{
    public class UserRelationshipService : IUserRelationshipService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UserRelationshipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ServiceResult> BlockUserAsync(string userId, string blockedUserId)
        {
            var (orderedUseId1, orderedUserId2) = UserHelpers.GetOrderedUserIds(userId, blockedUserId);

            var relationship = await unitOfWork.UserRelationships.GetFriendshipAsync(userId, blockedUserId);

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

                await unitOfWork.UserRelationships.AddAsync(relationship);
            }
            else
            {
                relationship.Status = RelationshipStatusEnum.Blocked;
                relationship.ActionUserId = userId;

                unitOfWork.UserRelationships.Update(relationship);
            }

            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: "User was blocked");
        }

        public async Task<ServiceResult> GetFriendRequestsAsync(string userId, int pageNubmer, int pageSize)
        {
            var requests = await unitOfWork.UserRelationships.GetFriendRequestsAsync(userId, pageNubmer, pageSize);

            var requestsDTO = mapper.Map<IEnumerable<FriendRequestDTO>>(requests);

            return new ServiceResult(true, data: requestsDTO);
        }

        public async Task<ServiceResult> GetFriendsAsync(string userId, int pageNubmer, int pageSize)
        {
            var friends = await unitOfWork.UserRelationships.GetFriendsAsync(userId, pageNubmer, pageSize);

            var friendsDTO = mapper.Map<List<RelationMemberDTO>>(friends, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return new ServiceResult(true, data: friendsDTO);
        }

        public async Task<ServiceResult> RemoveFriendAsync(string userId, string friendUserId)
        {
            var (orderedUserId1, orderedUserId2) = UserHelpers.GetOrderedUserIds(userId, friendUserId);

            var relationship = await unitOfWork.UserRelationships.GetFriendshipAsync(orderedUserId1, orderedUserId2);

            if (relationship is null || relationship.Status is not RelationshipStatusEnum.Accepted)
                return new ServiceResult(false, ["Users are not friends"]);

            await unitOfWork.UserRelationships.RemoveAsync(r => r.Id == relationship.Id);
            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: "Friend was removed");
        }

        public async Task<ServiceResult> RespondToFriendRequestAsync(int requestId, string responderUserId, string action)
        {
            var relationship = await unitOfWork.UserRelationships.GetSingleAsync(r => r.Id == requestId);

            if (relationship is null || relationship.Status is not RelationshipStatusEnum.Pending)
                return new ServiceResult(false, ["Request does not exist"]);

            if (relationship.User1Id != responderUserId && relationship.User2Id != responderUserId)
                return new ServiceResult(false, ["Not authorized to respond to this friend request"]);

            if (action.Equals("Accept", StringComparison.OrdinalIgnoreCase))
            {
                relationship.Status = RelationshipStatusEnum.Accepted;
                relationship.ActionUserId = responderUserId;

                unitOfWork.UserRelationships.Update(relationship);
                await unitOfWork.CommitAsync();

                return new ServiceResult(true, data: "Request was accepted");
            }
            else if (action.Equals("Decline", StringComparison.OrdinalIgnoreCase))
            {
                relationship.Status = RelationshipStatusEnum.Declined;
                relationship.ActionUserId = responderUserId;

                await unitOfWork.UserRelationships.RemoveAsync(r => r.Id == relationship.Id);
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

            var existingFriendship = await unitOfWork.UserRelationships.GetFriendshipAsync(orderedUserId1, orderedUserId2);

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

            await unitOfWork.UserRelationships.AddAsync(friendship);
            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: "Request was sent");
        }

        public async Task<ServiceResult> UnblockUserAsync(string userId, string blockedUserId)
        {
            var (orderedUserId1, orderedUserId2) = UserHelpers.GetOrderedUserIds(userId, blockedUserId);

            var relationship = await unitOfWork.UserRelationships.GetFriendshipAsync(orderedUserId1, orderedUserId2);

            if (relationship is null || relationship.Status is not RelationshipStatusEnum.Blocked)
                return new ServiceResult(false, ["User is not blocked"]);

            await unitOfWork.UserRelationships.RemoveAsync(r => r.Id == relationship.Id);
            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: "User was Unblocked");
        }

        public async Task<ServiceResult> GetBlockedUsers(string userId, int pageNubmer, int pageSize)
        {
            var blockedUsers = await unitOfWork.UserRelationships.GetBlockedUsersAsync(userId, pageNubmer, pageSize);

            var blockedUsersDTO = mapper.Map<List<RelationMemberDTO>>(blockedUsers, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return new ServiceResult(true, data: blockedUsersDTO);
        }
    }
}
