using System;
using System.Collections.Generic;
using Example009.HWR.Core;

namespace Example009.HWR.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
        TeamId IssuerTeam { get; }
        Tick IssuedAtTick { get; }
    }

    public sealed class MoveCommand : ICommand
    {
        public MoveCommand(Guid id, TeamId issuerTeam, Tick issuedAtTick, IReadOnlyList<EntityId> units, Float3 destination)
        {
            Id = id;
            IssuerTeam = issuerTeam;
            IssuedAtTick = issuedAtTick;
            Units = units;
            Destination = destination;
        }

        public Guid Id { get; }
        public TeamId IssuerTeam { get; }
        public Tick IssuedAtTick { get; }
        public IReadOnlyList<EntityId> Units { get; }
        public Float3 Destination { get; }
    }

    public sealed class AttackCommand : ICommand
    {
        public AttackCommand(Guid id, TeamId issuerTeam, Tick issuedAtTick, IReadOnlyList<EntityId> units, EntityId target)
        {
            Id = id;
            IssuerTeam = issuerTeam;
            IssuedAtTick = issuedAtTick;
            Units = units;
            Target = target;
        }

        public Guid Id { get; }
        public TeamId IssuerTeam { get; }
        public Tick IssuedAtTick { get; }
        public IReadOnlyList<EntityId> Units { get; }
        public EntityId Target { get; }
    }

    public sealed class StopCommand : ICommand
    {
        public StopCommand(Guid id, TeamId issuerTeam, Tick issuedAtTick, IReadOnlyList<EntityId> units)
        {
            Id = id;
            IssuerTeam = issuerTeam;
            IssuedAtTick = issuedAtTick;
            Units = units;
        }

        public Guid Id { get; }
        public TeamId IssuerTeam { get; }
        public Tick IssuedAtTick { get; }
        public IReadOnlyList<EntityId> Units { get; }
    }

    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        IReadOnlyList<ICommand> DrainForTick(Tick tick);
    }
}
