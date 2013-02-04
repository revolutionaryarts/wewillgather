using System.Web;
using System.Web.Mvc;
using Gather.Services.Pages;
using Gather.Web.Extensions;
using Gather.Web.Models.Page;

namespace Gather.Web.Controllers
{
    public class StaticController : BaseController
    {

        #region Fields

        private readonly IPageService _pageService;        

        #endregion

        #region Constructor

        public StaticController (IPageService pageService)
        {
            _pageService = pageService;            
        }

        #endregion

        #region Methods

        public ActionResult Detail(int id)
        {
            var model = PrepareStaticPageModel(id);

            AddHomeBreadcrumb();
            AddBreadcrumb(model.Title, Url.Action("Detail", "Static", new { Id = id }));

            return View("StandardPage", model);
        }

        #endregion

        #region Prepare page model

        private PageModel PrepareStaticPageModel(int pageId)
        {
            var model = _pageService.GetPageById(pageId).ToModel();

            if (model == null)
                throw new HttpException(404, "not found");

            if (model.Deleted || !model.Active)
                throw new HttpException(404, "not found");

            return model;
        }

        #endregion

    }
}