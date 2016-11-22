using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NEventLite.Command_Bus;
using NEventLite.Extensions;
using NEventLite_Example.Commands;
using NEventLite_Example.Read_Model;
using NEventLite_Example_MVC.Models;
using WebGrease.Css.Ast;

namespace NEventLite_Example_MVC.Controllers
{
    public class NoteDtoController : Controller
    {
        // GET: NoteDto
        public ActionResult Index()
        {
            return View(GetReadRepository().GetAllNotes().OrderBy(o => o.CreatedDate).Select(o => new NoteDto(o)));
        }

        // GET: NoteDto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NoteDto/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {

            if (ModelState.IsValid)
            {
                var result = await GetCommandBus().ExecuteAsync(
                    new CreateNoteCommand(Guid.NewGuid(), Guid.NewGuid(), -1, collection["Title"],
                        collection["Description"], collection["Category"]));

                result.EnsurePublished();

                //Let events commit
                await Task.Delay(200);
            }

            return RedirectToAction("Index");

        }

        // GET: NoteDto/Edit/5
        public ActionResult Edit(Guid id)
        {
            var note = new NoteDto(GetReadRepository().GetNote(id));

            return View(note);
        }

        // POST: NoteDto/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Guid id, int CurrentVersion, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var result = await GetCommandBus().ExecuteAsync(
                    new EditNoteCommand(Guid.NewGuid(), id, CurrentVersion, collection["Title"],
                        collection["Description"], collection["Category"]));

                result.EnsurePublished();

                //Let events commit
                await Task.Delay(200);
            }

            return RedirectToAction("Index");
        }

        private MyReadRepository GetReadRepository()
        {
            return ((MvcApplication)this.HttpContext.ApplicationInstance).GetDependencyResolver().Resolve<MyReadRepository>();
        }

        private ICommandBus GetCommandBus()
        {
            return ((MvcApplication)this.HttpContext.ApplicationInstance).GetDependencyResolver().Resolve<ICommandBus>();
        }

    }
}
