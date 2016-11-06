using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Command_Handlers;
using NEventLite.Repository;
using NEventLite_Example.Commands;
using NEventLite_Example.Domain;

namespace NEventLite_Example.Command_Handlers
{
    public class CreateItemCommandHandler:ICommandHandler<CreateNoteCommand>
    {
        private IRepository<Note> _repository;

        public CreateItemCommandHandler(IRepository<Note> repository)
        {
            _repository = repository;
        }

        public void Handle(CreateNoteCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
