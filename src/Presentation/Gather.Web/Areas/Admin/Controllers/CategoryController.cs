using System;
using System.Linq;
using System.Web.Mvc;
using Gather.Core.Domain.Common;
using Gather.Services.Security;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Services.Categories;
using Gather.Web.Areas.Admin.Models.Category;
using Gather.Core.Domain.Categories;
using Gather.Web.Models.Category;
using Gather.Core;
using Gather.Services.Projects;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class CategoryController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly ICategoryService _categoryService;
        private readonly IPermissionService _permissionService;
        private readonly IProjectService _projectService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public CategoryController() { }

        public CategoryController(CoreSettings coreSettings, IPermissionService permissionService, ICategoryService categoryService, IWorkContext workContext, IProjectService projectService)
        {
            _coreSettings = coreSettings;
            _categoryService = categoryService;
            _permissionService = permissionService;
            _projectService = projectService;
            _workContext = workContext;
        }

        #endregion

        #region Methods
        
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageCategories))
                return AccessDeniedView();

            var categories = _categoryService.GetAllCategories(Page, _coreSettings.AdminGridPageSize, null, Search);

            var model = new CategoryListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = categories.TotalCount,
                TotalPages = categories.TotalPages,
				Search = Search,
                Categories = categories.Select(PrepareListCategoryModel).ToList()
            };

            PrepareBreadcrumbs();
            return View(model);
        }

        public ActionResult Add()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageCategories))
                return AccessDeniedView();

            PrepareBreadcrumbs();
            AddBreadcrumb("Add New Category", null);

            return View(new CategoryModel());
        }

        [HttpPost]
        [FormValueRequired("add")]
        public ActionResult Add(CategoryModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageCategories))
                return AccessDeniedView();

            Category category = new Category
            {
                CreatedBy = _workContext.CurrentUser.Id,
                LastModifiedBy = _workContext.CurrentUser.Id
            };

            if (ModelState.IsValid)
            {
                try
                {
                    category.Name = model.Name;
                    _categoryService.InsertCategory(category);

                    SuccessNotification("The category details have been added successfully.");
                    return RedirectToAction("index");
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the category details, please try again.");
                }
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Add New Category", null);

            return View(model);
        }

        public ActionResult AdditionalDelete(int id)
        {
            var model = new AdditionalDeleteModel
            {
                Categories = _categoryService.GetAllCategories(0, -1, null).Select(x => x.ToModel()).ToList(),
                CategoryId = id
            };
            model.Categories.First(m => m.Id == id).Name = "Un-assign";
            return PartialView("_AdditionalDelete", model);
        }

        public ActionResult Delete(int id, AdditionalDeleteModel deleteModel)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(id);
            if (category == null)
                return RedirectToAction("index", new { page = Page, search = Search });

            try
            {
                var projects = _projectService.GetAllProjectsByCategoryId(id);
                if(id == deleteModel.CategoryId)
                {
                    foreach (var project in projects )
                    {
                        project.Categories.Remove(project.Categories.First(c => c.Id == id));
                    }
                }
                else
                {
                    foreach (var project in projects)
                    {
                        project.Categories.Remove(project.Categories.First(c => c.Id == id));
                        if (project.Categories.FirstOrDefault(c => c.Id == deleteModel.CategoryId) == null)
                            project.Categories.Add(_categoryService.GetCategoryById(deleteModel.CategoryId));
                    }
                }

                _projectService.BulkUpdateProjects(projects);
                _categoryService.DeleteCategory(category);
                SuccessNotification(category.Name + " has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting " + category.Name + ", please try again");
            }

            return RedirectToAction("index", new { page = Page, search = Search });
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageCategories))
                return AccessDeniedView();

            // get the category
            var category = _categoryService.GetCategoryById(id);

            // check we have a category and they are not deleted
            if (category == null || category.Deleted)
                return RedirectToAction("index");

            if (!category.Active)
                WarningNotification("This category is currently hidden. To show it, use the 'show' button on the right.");

            PrepareBreadcrumbs();
            AddBreadcrumb(category.Name, null);

            var model = category.ToModel();
            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(CategoryModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageCategories))
                return AccessDeniedView();

            // get the category
            var category = _categoryService.GetCategoryById(model.Id);

            // check we have a category and they are not deleted
            if (category == null || category.Deleted)
                return RedirectToAction("index");
            category.LastModifiedBy = _workContext.CurrentUser.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    category.Name = model.Name;
                    _categoryService.UpdateCategory(category);

                    SuccessNotification("The category details have been updated successfully.");
                    return RedirectToAction("edit", category.Id);
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the category details, please try again.");
                }
            }
            else
            {
                ErrorNotification("We were unable to make the change, please review the form and correct the errors.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb(category.Name, null);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("hide")]
        public ActionResult Hide(CategoryModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageCategories))
                return AccessDeniedView();

            // get the category
            var category = _categoryService.GetCategoryById(model.Id);

            // check we have a category and they are not deleted
            if (category == null || category.Deleted)
                return RedirectToAction("index");
            category.LastModifiedBy = _workContext.CurrentUser.Id;

            try
            {
                category.Active = !category.Active;
                _categoryService.UpdateCategory(category);

                SuccessNotification(category.Active ? "The category has been shown successfully." : "The category has been hidden successfully.");

                return RedirectToAction("edit", category.Id);
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred hiding the category, please try again.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Category", null);

            return View(model);
        }

        private void PrepareBreadcrumbs()
        {
            AddBreadcrumb("Actions", Url.Action("index", "project"));
            AddBreadcrumb("Categories", Url.Action("index"));
        }

        #endregion

        #region Prepare Models

        [NonAction]
        private CategoryModel PrepareListCategoryModel(Category category)
        {
            var model = category.ToModel();

            model.Actions.Add(new ModelActionLink
            {
                Alt = "Edit",
                Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                Target = Url.Action("edit", new { id = category.Id })
            });

            model.Actions.Add(new DeleteActionLink(category.Id, Search, Page));

            return model;
        }

        #endregion

    }
}