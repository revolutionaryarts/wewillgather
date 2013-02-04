using System.Drawing;
using System.IO;
using System.Linq;
using FluentValidation;
using Gather.Services.SuccessStories;
using Gather.Web.Models.SuccessStory;

namespace Gather.Web.Validators.SuccessStory
{
    public class SuccessStoryValidator : AbstractValidator<SuccessStoryModel>
    {
        public SuccessStoryValidator(ISuccessStoryService successStoryService)
        {
            RuleFor(x => x.Title).Must((x, u) =>
            {
                if (successStoryService.GetAllSuccessStories().Any(p => p.Id != x.Id && p.Title.Trim().ToLower() == x.Title.Trim().ToLower()))
                    return false;
                return true;
            }).WithMessage("The Success Story title must be unique");
            
            RuleFor(x => x.UploadedFile).Must((x, u) =>
            {
                if ((u == null || u.ContentLength == 0) && x.Id == 0)
                    return false;
                return true;
            }).WithMessage("The Image field is required.")
            .Must((x, u) =>
            {
                if (u != null && u.ContentLength > 0)
                {
                    string fileExtension = (Path.GetExtension(u.FileName) ?? "").ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                        return false;
                }
                return true;
            }).WithMessage("An Error occurred, please only upload .jpg or .png file types.")
            .Must((x, u) =>
            {
                if (u != null && u.ContentLength > 0)
                {
                     string fileExtension = (Path.GetExtension(u.FileName) ?? "").ToLower();
                     if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                     {
                         using (var input = new Bitmap(u.InputStream))
                         {
                             if (input.Width != 1000 && input.Height != 640)
                                 return false;
                         }
                     }
                }
                return true;
            }).WithMessage("Please ensure the dimensions of the image are 1000x640px.");
        }
    }
}