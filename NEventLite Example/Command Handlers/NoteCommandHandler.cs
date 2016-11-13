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
using NEventLite.Unit_Of_Work;
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
            var work = new UnitOfWork<Note>(_repository);
            var newNote = new Note(command.AggregateId, command.title, command.desc, command.cat);

            work.Add(newNote);
            work.Commit();

            return new CommandResult(newNote.CurrentVersion, true, "");
        }

        public ICommandResult Handle(EditNoteCommand command)
        {
            var work = new UnitOfWork<Note>(_repository);
            var loadedNote = work.Get(command.AggregateId,command.TargetVersion);

            loadedNote.ChangeTitle(command.title);
            loadedNote.ChangeCategory(command.cat);

            work.Commit();

            return new CommandResult(loadedNote.CurrentVersion, true, "");
        }

    }
}
