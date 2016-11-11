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
            var loadedNote = _repository.EnsureExists(command.AggregateId, command.TargetVersion);

            loadedNote.ChangeTitle(command.title);
            loadedNote.ChangeCategory(command.cat);

            _repository.Save(loadedNote);

            return new CommandResult(loadedNote.CurrentVersion, true, "");
        }
    }
}
