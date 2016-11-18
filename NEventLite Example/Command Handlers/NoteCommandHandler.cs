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

        public async Task<ICommandResult> Handle(CreateNoteCommand command)
        {
            var work = new UnitOfWork(_repository);
            var newNote = new Note(command.AggregateId, command.Title, command.Desc, command.Cat);

            var task = await work.AddAsync(newNote).ContinueWith((o) => work.CommitAsync());

            return new CommandResult(newNote.CurrentVersion, task.IsFaulted == false, task.Exception?.Message);
        }

        public async Task<ICommandResult> Handle(EditNoteCommand command)
        {
            var work = new UnitOfWork(_repository);
            var loadedNote = await work.GetAsync<Note>(command.AggregateId, command.TargetVersion);

            loadedNote.ChangeTitle(command.Title);
            loadedNote.ChangeDescription(command.Description);
            loadedNote.ChangeCategory(command.Cat);

            var task = work.CommitAsync();
            await task;

            return new CommandResult(loadedNote.CurrentVersion, task.IsFaulted == false, task.Exception?.Message);
        }

    }
}
