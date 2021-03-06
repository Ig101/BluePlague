using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Game.Entities;
using ProjectArena.Domain.QueueService;
using ProjectArena.Infrastructure.Models.ErrorHandling;
using ProjectArena.Infrastructure.Models.Game;

namespace ProjectArena.Application.Game.Commands.HirePatron
{
    public class HirePatronCommand : IRequest<NewCharacterDto>
    {
        public string UserId { get; set; }

        public int PatronId { get; set; }

        public string Name { get; set; }

        public string CharacterForReplace { get; set; }

        internal class Handler : IRequestHandler<HirePatronCommand, NewCharacterDto>
        {
            private readonly GameContext _gameContext;
            private readonly IQueueService _queueService;
            private readonly IBattleService _battleService;

            public Handler (
                GameContext gameContext,
                IQueueService queueService,
                IBattleService battleService)
            {
                _gameContext = gameContext;
                _queueService = queueService;
                _battleService = battleService;
            }

            public async Task<NewCharacterDto> Handle(HirePatronCommand request, CancellationToken cancellationToken)
            {
                if (_queueService.IsUserInQueue(request.UserId) != null || _battleService.IsUserInBattle(request.UserId))
                {
                    throw new CannotPerformOperationException("User cannot do that in queue or battle.");
                }

                var roster = await _gameContext.Rosters.GetOneAsync(x => x.UserId == request.UserId);
                var characters = await _gameContext.Characters.GetAsync(x => x.RosterId == roster.Id);
                if (characters.Count() < 3 && request.CharacterForReplace != null)
                {
                    throw new CannotPerformOperationException("Cannot fire character while roster is not full.");
                }

                if (characters.Count() >= 3 && request.CharacterForReplace == null)
                {
                    throw new CannotPerformOperationException("Need character to fire while roster is not full.");
                }

                if (request.CharacterForReplace != null && !characters.Any(x => x.Id == request.CharacterForReplace))
                {
                    throw new CannotPerformOperationException("Firing character is not yours.");
                }

                if (roster.BoughtPatrons.Contains(request.PatronId) || request.PatronId > roster.TavernCapacity || request.PatronId < 1)
                {
                    throw new CannotPerformOperationException("Patron has already been hired.");
                }

                var characterForReplace = characters.FirstOrDefault(x => x.Id == request.CharacterForReplace);

                if (request.CharacterForReplace != null && characterForReplace.IsKeyCharacter)
                {
                    throw new CannotPerformOperationException("Cannot fire key character");
                }

                _gameContext.Rosters.Update(
                    x => x.UserId == roster.UserId,
                    Builders<Roster>.Update.Set(x => x.BoughtPatrons, roster.BoughtPatrons.Append(request.PatronId)));
                var character = new Character()
                {
                    Id = characterForReplace != null ? characterForReplace.Id : Guid.NewGuid().ToString(),
                    Deleted = false,
                    IsKeyCharacter = false,
                    RosterId = roster.Id,
                    Name = request.Name.Trim(),
                    ChosenTalents = new int[0]
                };
                if (request.CharacterForReplace != null)
                {
                    _gameContext.Characters.ReplaceOne(x => x.Id == request.CharacterForReplace, character, true);
                }
                else
                {
                    _gameContext.Characters.InsertOne(character);
                }

                await _gameContext.ApplyChangesAsync(cancellationToken);
                return new NewCharacterDto()
                {
                    Id = character.Id
                };
            }
        }
    }
}