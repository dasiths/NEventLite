using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Commands;
using NEventLite.Command_Handlers;
using NEventLite.Exceptions;
using NEventLite.Extensions;
using NEventLite.Repository;
using NEventLite_Example.Commands;
using NEventLite_Example.Domain;
using NEventLite_Example.Repository;

namespace NEventLite_Example.Command_Handlers
{
    public class NoteCommandHandler : ICommandHandler<CreateNoteCommand>,
                                      ICommandHandler<EditNoteCommand>
    {
        private readonly NoteRepository _repository;
        public NoteCommandHandler(NoteRepository repository)
        {
            _repository = repository;
        }

        public ICommandResult Handle(CreateNoteCommand command)
        {
            _repository.EnsureDoesntExist(command.AggregateId);

            var newNote = new Note(command.AggregateId, command.title, command.desc, command.cat);
            _repository.Save(newNote);

            return new CommandResult(newNote.CurrentVersion, true, "");
        }

        public ICommandResult Handle(EditNoteCommand command)
        {
            var LoadedNote = _repository.EnsureExists(command.AggregateId);
            LoadedNote.EnsureVersionMatch(command.TargetVersion);

            if (LoadedNote.Title != command.title)
            {
                LoadedNote.ChangeTitle(command.title);
            }

            if (LoadedNote.Category != command.cat)
            {
                LoadedNote.ChangeCategory(command.cat);
            }

            _repository.Save(LoadedNote);

            return new CommandResult(LoadedNote.CurrentVersion, true, "");
        }
    }
}
