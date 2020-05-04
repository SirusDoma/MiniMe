﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMe.Chunithm.Comparers;
using MiniMe.Chunithm.Data;
using MiniMe.Chunithm.Data.Models;
using MiniMe.Chunithm.Protocols;
using MiniMe.Core.Mapper;
using MiniMe.Core.Repositories;
using Serilog;
using DbUserGameOption = MiniMe.Chunithm.Data.Models.UserGameOption;
using DbUserGameOptionEx = MiniMe.Chunithm.Data.Models.UserGameOptionEx;
using DbUserMap = MiniMe.Chunithm.Data.Models.UserMap;
using DbUserCharacter = MiniMe.Chunithm.Data.Models.UserCharacter;
using DbUserItem = MiniMe.Chunithm.Data.Models.UserItem;
using DbUserMusic = MiniMe.Chunithm.Data.Models.UserMusic;
using DbUserActivity = MiniMe.Chunithm.Data.Models.UserActivity;
using DbUserPlayLog = MiniMe.Chunithm.Data.Models.UserPlayLog;
using DbUserCourse = MiniMe.Chunithm.Data.Models.UserCourse;
using DbUserDataEx = MiniMe.Chunithm.Data.Models.UserDataEx;
using DbUserDuelList = MiniMe.Chunithm.Data.Models.UserDuelList;
using UserActivity = MiniMe.Chunithm.Protocols.UserActivity;
using UserCharacter = MiniMe.Chunithm.Protocols.UserCharacter;
using UserCourse = MiniMe.Chunithm.Protocols.UserCourse;
using UserDataEx = MiniMe.Chunithm.Protocols.UserDataEx;
using UserDuelList = MiniMe.Chunithm.Protocols.UserDuelList;
using UserGameOption = MiniMe.Chunithm.Protocols.UserGameOption;
using UserGameOptionEx = MiniMe.Chunithm.Protocols.UserGameOptionEx;
using UserItem = MiniMe.Chunithm.Protocols.UserItem;
using UserMap = MiniMe.Chunithm.Protocols.UserMap;
using UserMusic = MiniMe.Chunithm.Protocols.UserMusic;
using UserProfile = MiniMe.Chunithm.Protocols.UserProfile;

namespace MiniMe.Chunithm.Controllers
{
    [ApiController]
    [Route("/ChuniServlet")]
    public class ServletController : ControllerBase
    {
        private readonly ChunithmContext _context;
        private readonly IAimeService _aimeService;

        public ServletController(ChunithmContext context, IAimeService aimeService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _aimeService = aimeService ?? throw new ArgumentNullException(nameof(aimeService));
        }

        [HttpPost("GameLoginApi")]
        public GameLoginResponse GameLogin( /* GameLoginRequest request */)
        {
            return new GameLoginResponse
            {
                ReturnCode = 1
            };
        }

        [HttpPost("GameLogoutApi")]
        public GameLogoutResponse GameLogout( /* GameLogoutRequest request */)
        {
            return new GameLogoutResponse
            {
                ReturnCode = 1
            };
        }

        [HttpPost("GetGameChargeApi")]
        public GetGameChargeResponse GetGameCharge( /* GetGameChargeRequest request */)
        {
            return new GetGameChargeResponse
            {
                Length = 0,
                GameChargeList = Array.Empty<GameChargeItem>()
            };
        }

        [HttpPost("GetGameEventApi")]
        public GetGameEventResponse GetGameEvent(GetGameEventRequest request)
        {
            var startDate = new DateTime(2017, 12, 5, 7, 0, 0);
            var endDate = new DateTime(2099, 12, 31, 0, 0, 0);

            GameEvent[] events = ChunithmEvents.EventIds
                .Select(id => new GameEvent
                {
                    Type = request.Type,
                    Id = id,
                    StartDate = startDate,
                    EndDate = endDate
                })
                .ToArray();

            return new GetGameEventResponse
            {
                Type = request.Type,
                Length = events.Length,
                GameEventList = events
            };
        }

        [HttpPost("GetGameIdlistApi")]
        public GetGameIdlistResponse GetGameIdlist(GetGameIdlistRequest request)
        {
            return new GetGameIdlistResponse
            {
                Type = request.Type,
                Length = 0,
                GameIdlistList = Array.Empty<object>()
            };
        }

        [HttpPost("GetGameMessageApi")]
        public GetGameMessageResponse GetGameMessage(GetGameMessageRequest request)
        {
            return new GetGameMessageResponse
            {
                Type = request.Type,
                Length = 0,
                GameMessageList = Array.Empty<GameMessage>()
            };
        }

        [HttpPost("GetGameRankingApi")]
        public GetGameRankingResponse GetGameRanking(GetGameRankingRequest request)
        {
            return new GetGameRankingResponse
            {
                Type = request.Type,
                GameRankingList = Array.Empty<GameRanking>()
            };
        }

        [HttpPost("GetGameSaleApi")]
        public GetGameSaleResponse GetGameSale(GetGameSaleRequest request)
        {
            return new GetGameSaleResponse
            {
                Type = request.Type,
                Length = 0,
                GameSaleList = Array.Empty<GameSale>()
            };
        }

        [HttpPost("GetGameSettingApi")]
        public GetGameSettingResponse GetGameSetting( /* GetGameSettingRequest request */)
        {
            return new GetGameSettingResponse
            {
                GameSetting = new GetGameSettingResponse.Payload
                {
                    DataVersion = "1",
                    IsMaintenance = false,
                    RequestInterval = 10,
                    RebootStartTime = DateTime.Now.AddHours(-3),
                    RebootEndTime = DateTime.Now.AddHours(-2),
                    IsBackgroundDistribute = false,
                    MaxCountCharacter = 300,
                    MaxCountItem = 300,
                    MaxCountMusic = 300
                },
                IsDumpUpload = false,
                IsAou = false
            };
        }

        [HttpPost("GetUserActivityApi")]
        public GetUserActivityResponse GetUserActivity(GetUserActivityRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.Activities);

            UserActivity[] activities = Array.Empty<UserActivity>();

            if (profile != null)
            {
                activities = profile.Activities
                    .Where(a => a.Kind == request.Kind)
                    .Select(ObjectMapper.Map<UserActivity>)
                    .ToArray();
            }

            return new GetUserActivityResponse
            {
                UserId = request.UserId,
                Kind = request.Kind,
                Length = activities.Length,
                UserActivityList = activities
            };
        }

        [HttpPost("GetUserCharacterApi")]
        public GetUserCharacterResponse GetUserCharacter(GetUserCharacterRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.Characters);

            UserCharacter[] characters = Array.Empty<UserCharacter>();

            if (profile != null)
            {
                characters = profile.Characters
                    .Skip(request.NextIndex)
                    .Take(request.MaxCount)
                    .Select(ObjectMapper.Map<UserCharacter>)
                    .ToArray();
            }

            return new GetUserCharacterResponse
            {
                UserId = request.UserId,
                NextIndex = ChunithmUtility.NextPagination(characters, request),
                Length = characters.Length,
                UserCharacterList = characters
            };
        }

        [HttpPost("GetUserChargeApi")]
        public GetUserChargeResponse GetUserCharge(GetUserChargeRequest request)
        {
            return new GetUserChargeResponse
            {
                UserId = request.UserId,
                Length = 0,
                UserChargeList = Array.Empty<object>()
            };
        }

        [HttpPost("GetUserCourseApi")]
        public GetUserCourseResponse GetUserCourse(GetUserCourseRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.Courses);

            UserCourse[] courses = Array.Empty<UserCourse>();

            if (profile != null)
            {
                courses = profile.Courses
                    .Skip(request.NextIndex)
                    .Take(request.MaxCount)
                    .Select(ObjectMapper.Map<UserCourse>)
                    .ToArray();
            }

            return new GetUserCourseResponse
            {
                UserId = request.UserId,
                NextIndex = ChunithmUtility.NextPagination(courses, request),
                Length = courses.Length,
                UserCourseList = courses
            };
        }

        [HttpPost("GetUserDataApi")]
        public GetUserDataResponse GetUserData(GetUserDataRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithAllData(aimeId);

            return new GetUserDataResponse
            {
                UserId = request.UserId,
                UserData = ObjectMapper.Map<UserProfile>(profile)
            };
        }

        [HttpPost("GetUserDataExApi")]
        public GetUserDataExResponse GetUserDataEx(GetUserDataExRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.DataEx);

            return new GetUserDataExResponse
            {
                UserId = request.UserId,
                UserDataEx = ObjectMapper.Map<UserDataEx>(profile?.DataEx)
            };
        }

        [HttpPost("GetUserDuelApi")]
        public GetUserDuelResponse GetUserDuel(GetUserDuelRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.DuelLists);

            UserDuelList[] duelLists = Array.Empty<UserDuelList>();

            if (profile != null)
            {
                duelLists = profile.DuelLists
                    .Select(ObjectMapper.Map<UserDuelList>)
                    .ToArray();
            }

            return new GetUserDuelResponse
            {
                UserId = request.UserId,
                Length = duelLists.Length,
                UserDuelList = duelLists
            };
        }

        [HttpPost("GetUserItemApi")]
        public GetUserItemResponse GetUserItem(GetUserItemRequest request)
        {
            // Split the "nextIndex" parameter

            const long itemKindMul = 10000000000;

            int itemKind = (int)(request.NextIndex / itemKindMul);
            int nextIndex = (int)(request.NextIndex % itemKindMul);

            // Hit DB
            // This gets called for not-yet-created profiles for some reason (probably
            // so that the server can force some bonus items into a new profile's
            // inventory)? So we need to gracefully handle that condition.

            // (maybe some sort of anchor row is created on first GameLogin...)

            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.Items);

            UserItem[] items = Array.Empty<UserItem>();

            if (profile != null)
            {
                items = profile.Items
                    .Where(i => i.ItemKind == itemKind)
                    .Skip(nextIndex)
                    .Take(request.MaxCount)
                    .Select(ObjectMapper.Map<UserItem>)
                    .ToArray();
            }

            // Pack the next pagination cookie

            long xout = itemKind * itemKindMul + nextIndex + items.Length;

            return new GetUserItemResponse
            {
                UserId = request.UserId,
                Length = items.Length,
                NextIndex = items.Length < request.MaxCount ? -1 : xout,
                ItemKind = itemKind,
                UserItemList = items
            };
        }

        [HttpPost("GetUserMapApi")]
        public GetUserMapResponse GetUserMap(GetUserMapRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.Maps);

            UserMap[] maps = Array.Empty<UserMap>();

            if (profile != null)
            {
                maps = profile.Maps
                    .Select(ObjectMapper.Map<UserMap>)
                    .ToArray();
            }

            return new GetUserMapResponse
            {
                UserId = request.UserId,
                Length = maps.Length,
                UserMapList = maps
            };
        }

        [HttpPost("GetUserMusicApi")]
        public GetUserMusicResponse GetUserMusic(GetUserMusicRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.Musics);

            UserMusic[] musics = Array.Empty<UserMusic>();

            if (profile != null)
            {
                musics = profile.Musics
                    .Distinct(UserMusicEqualityComparer.Instance)
                    .Skip(request.NextIndex)
                    .Take(request.MaxCount)
                    .Select(ObjectMapper.Map<UserMusic>)
                    .ToArray();
            }

            return new GetUserMusicResponse
            {
                UserId = request.UserId,
                NextIndex = ChunithmUtility.NextPagination(musics, request),
                Length = musics.Length,
                UserMusicList = musics
            };
        }

        [HttpPost("GetUserOptionApi")]
        public GetUserOptionResponse GetUserOption(GetUserOptionRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.GameOption);

            return new GetUserOptionResponse
            {
                UserId = request.UserId,
                UserGameOption = ObjectMapper.Map<UserGameOption>(profile?.GameOption)
            };
        }

        [HttpPost("GetUserOptionExApi")]
        public GetUserOptionExResponse GetUserOptionEx(GetUserOptionExRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.GameOptionEx);

            return new GetUserOptionExResponse
            {
                UserId = request.UserId,
                UserGameOptionEx = ObjectMapper.Map<UserGameOptionEx>(profile?.GameOptionEx)
            };
        }

        [HttpPost("GetUserPreviewApi")]
        public GetUserPreviewResponse GetUserPreview(GetUserPreviewRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);

            var profile = _context.FindProfileWithQuery(aimeId, ps => ps
                .Include(p => p.Characters)
                .Include(p => p.GameOption));

            if (profile == null)
            {
                return null;
            }

            var character = profile.Characters.First();
            var gameOption = profile.GameOption;

            return new GetUserPreviewResponse
            {
                UserId = 1,

                // Login status

                IsLogin = false,
                LastLoginDate = new DateTime(1970, 1, 1, 9, 0, 0),

                // UserData

                UserName = profile.UserName,
                ReincarnationNum = profile.ReincarnationNum,
                Level = profile.Level,
                Exp = profile.Exp,
                PlayerRating = profile.PlayerRating,
                LastGameId = profile.LastGameId,
                LastRomVersion = profile.LastRomVersion,
                LastDataVersion = profile.LastDataVersion,
                LastPlayDate = profile.LastPlayDate.DateTime,
                TrophyId = profile.TrophyId,

                // Selected UserCharacter

                UserCharacter = ObjectMapper.Map<UserCharacter>(character),

                // UserGameOption

                PlayerLevel = gameOption.PlayerLevel,
                Rating = gameOption.Rating,
                Headphone = gameOption.Headphone
            };
        }

        [HttpPost("GetUserRecentPlayerApi")]
        public GetUserRecentRatingResponse GetUserRecentPlayer(GetUserRecentRatingRequest request)
        {
            return GetUserRecentRating(request);
        }

        [HttpPost("GetUserRecentRatingApi")]
        public GetUserRecentRatingResponse GetUserRecentRating(GetUserRecentRatingRequest request)
        {
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);
            var profile = _context.FindProfileWithData(aimeId, p => p.PlayLogs);

            UserRecentRating[] logs = Array.Empty<UserRecentRating>();

            if (profile != null)
            {
                _context.Profiles
                    .Include(p => p.PlayLogs);

                logs = profile.PlayLogs
                    .OrderByDescending(l => l.UserPlayDate)
                    .Take(30)
                    .Select(l => new UserRecentRating
                    {
                        MusicId = l.MusicId,
                        DifficultId = l.Level,
                        // game version not saved in play log, just return a fixed version now
                        RomVersionCode = 1030000,
                        Score = l.Score
                    })
                    .ToArray();
            }

            return new GetUserRecentRatingResponse
            {
                UserId = request.UserId,
                Length = logs.Length,
                UserRecentRatingList = logs
            };
        }

        [HttpPost("GetUserRegionApi")]
        public GetUserRegionResponse GetUserRegion(GetUserRegionRequest request)
        {
            return new GetUserRegionResponse
            {
                UserId = request.UserId,
                Length = 0,
                UserRegionList = Array.Empty<UserRegion>()
            };
        }

        [HttpPost("UpsertClientBookkeepingApi")]
        public UpsertClientBookkeepingResponse UpsertClientBookkeeping( /* UpsertClientBookkeepingRequest request */)
        {
            return new UpsertClientBookkeepingResponse
            {
                ReturnCode = 1
            };
        }

        [HttpPost("UpsertClientDevelopApi")]
        public UpsertClientDevelopResponse UpsertClientDevelop( /* UpsertClientDevelopRequest request */)
        {
            return new UpsertClientDevelopResponse
            {
                ReturnCode = 1
            };
        }

        [HttpPost("UpsertClientErrorApi")]
        public UpsertClientErrorResponse UpsertClientError( /* UpsertClientErrorRequest request */)
        {
            return new UpsertClientErrorResponse
            {
                ReturnCode = 1
            };
        }

        [HttpPost("UpsertClientSettingApi")]
        public UpsertClientSettingResponse UpsertClientSetting( /* UpsertClientSettingRequest request */)
        {
            return new UpsertClientSettingResponse
            {
                ReturnCode = 1
            };
        }

        [HttpPost("UpsertClientTestmodeApi")]
        public UpsertClientTestmodeResponse UpsertClientTestmode( /* UpsertClientTestModeRequest request */)
        {
            return new UpsertClientTestmodeResponse
            {
                ReturnCode = 1
            };
        }

        [HttpPost("UpsertUserAllApi")]
        public UpsertUserAllResponse UpsertUserAll(UpsertUserAllRequest request)
        {
            if (request.UpsertUserAll == null)
            {
                // Invalid payload
                return new UpsertUserAllResponse();
            }

            var payload = request.UpsertUserAll;
            Guid? aimeId = _aimeService.FindIdByCardId(request.UserId);

            if (!aimeId.HasValue)
            {
                // AimeID not found
                return new UpsertUserAllResponse();
            }

            // Patch UserData.UserName (Double encoded UTF-8)
            var iso = Encoding.GetEncoding(28591);

            foreach (var userData in payload.UserData ?? Enumerable.Empty<UserProfile>())
            {
                userData.UserName = Encoding.UTF8.GetString(iso.GetBytes(userData.UserName));
            }

            // Upsert UserData(Profile)
            var profile = _context.FindProfile(aimeId);
            var isNew = false;

            if (profile == null)
            {
                if (payload.UserData?.Length > 0)
                {
                    // New user
                    isNew = true;
                    profile = ObjectMapper.Map<Data.Models.UserProfile>(payload.UserData[0]);
                    profile.Id = Guid.NewGuid();
                    profile.AimeId = aimeId!.Value;
                }
                else
                {
                    // ProfileId, UserData not found 
                    return new UpsertUserAllResponse();
                }
            }

            IEnumerable<DbUserGameOption> userGameOption = ChunithmUtility.PrepareDbObjects(
                ObjectMapper.Map<DbUserGameOption>(payload.UserGameOption),
                profile.Id);

            IEnumerable<DbUserGameOptionEx> userGameOptionEx = ChunithmUtility.PrepareDbObjects(
                ObjectMapper.Map<DbUserGameOptionEx>(payload.UserGameOptionEx),
                profile.Id);

            IEnumerable<DbUserDataEx> userDataEx = ChunithmUtility.PrepareDbObjects(
                ObjectMapper.Map<DbUserDataEx>(payload.UserDataEx),
                profile.Id);

            IEnumerable<DbUserMap> userMapList = PrepareProfileObjects(
                ObjectMapper.Map<DbUserMap>(payload.UserMapList),
                profile);

            IEnumerable<DbUserCharacter> userCharacterList = PrepareProfileObjects(
                ObjectMapper.Map<DbUserCharacter>(payload.UserCharacterList),
                profile);

            IEnumerable<DbUserItem> userItemList = PrepareProfileObjects(
                ObjectMapper.Map<DbUserItem>(payload.UserItemList),
                profile);

            IEnumerable<DbUserMusic> userMusicDetailList = PrepareProfileObjects(
                ObjectMapper.Map<DbUserMusic>(payload.UserMusicDetailList),
                profile);

            IEnumerable<DbUserActivity> userActivityList = PrepareProfileObjects(
                ObjectMapper.Map<DbUserActivity>(payload.UserActivityList),
                profile);

            IEnumerable<DbUserPlayLog> userPlayLogList = PrepareProfileObjects(
                ObjectMapper.Map<DbUserPlayLog>(payload.UserPlayLogList),
                profile);

            IEnumerable<DbUserCourse> userCourseList = PrepareProfileObjects(
                ObjectMapper.Map<DbUserCourse>(payload.UserCourseList),
                profile);

            IEnumerable<DbUserDuelList> userDuelList = PrepareProfileObjects(
                ObjectMapper.Map<DbUserDuelList>(payload.UserDuelList),
                profile);

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (payload.UserData?.Length > 0)
                {
                    // profile: tracking entity
                    ObjectMapper.Map(payload.UserData[0], profile);
                }

                if (isNew)
                {
                    _context.Profiles.Add(profile);
                    _context.SaveChanges();
                }

                _context.UpsertRange(userDataEx).On(o => o.Id).Run();
                _context.UpsertRange(userGameOption).On(o => o.Id).Run();
                _context.UpsertRange(userGameOptionEx).On(o => o.Id).Run();
                _context.UpsertRange(userActivityList).On(o => new { o.ProfileId, o.Kind, o.ActivityId }).Run();
                _context.UpsertRange(userCharacterList).On(o => new { o.ProfileId, o.CharacterId }).Run();
                _context.UpsertRange(userCourseList).On(o => new { o.ProfileId, o.CourseId }).Run();
                _context.UpsertRange(userDuelList).On(o => new { o.ProfileId, o.DuelId }).Run();
                _context.UpsertRange(userItemList).On(o => new { o.ProfileId, o.ItemKind, o.ItemId }).Run();
                _context.UpsertRange(userMapList).On(o => new { o.ProfileId, o.MapId }).Run();
                _context.UpsertRange(userMusicDetailList).On(o => new { o.ProfileId, o.MusicId, o.Level }).Run();
                _context.AddRange(userPlayLogList);
                _context.SaveChanges();

                transaction.Commit();

                return new UpsertUserAllResponse
                {
                    ReturnCode = 1
                };
            }
            catch (Exception e)
            {
                Log.Fatal(e, e.Message);

                transaction.Rollback();
                return new UpsertUserAllResponse();
            }
        }

        private IEnumerable<T> PrepareProfileObjects<T>(IEnumerable<T> objects, Data.Models.UserProfile profile) where T : IDbProfileObject
        {
            return objects.Select(obj =>
            {
                obj.Id = Guid.NewGuid();
                obj.Profile = profile;
                obj.ProfileId = profile.Id;
                return obj;
            });
        }
    }
}