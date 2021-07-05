using iPresenter;
using IronPython.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IO = System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using IronPython.Runtime;
using System.Diagnostics;
using System.IO;

namespace GraduationProjectPresentation.Pages
{
    public class SummarizeModel : PageModel
    {
        private IWebHostEnvironment _environment;
        public SummarizeModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [Required]
        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task<ActionResult> OnPostAsync()
        {
            var id = Guid.NewGuid().ToString();

            if (Upload == null)
            {
                ModelState.AddModelError("Upload File", "Please upload a valid pdf file.");
                return Page();
            }

            var extension = IO.Path.GetExtension(Upload.FileName);
            var file = IO.Path.Combine(_environment.WebRootPath, "uploads", id + extension);
            using (var fileStream = new IO.FileStream(file, IO.FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "C:/Users/Master/AppData/Local/Programs/Python/Python39/python.exe";
                start.Arguments = string.Format("{0} {1}", "fileProcessor.py", file);
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string jsonResult = reader.ReadToEnd().Split("#OUTPUT_START_HERE")[1];
                        var result = JsonSerializer.Deserialize<List<string>>(jsonResult);
                        var summarizedPath = result[0];
                        var summarized = await IO.File.ReadAllTextAsync(summarizedPath);

                        var imagesPath = result[1];
                        var images = await IO.File.ReadAllTextAsync(imagesPath);

                        var summarizedText = JsonSerializer.Deserialize<SummarizedText>(summarized, options: new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        var imageParts = JsonSerializer.Deserialize<List<ImagePart>>(images, options: new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        SmarterPresenter presenter = new();
                        var presentation = presenter.ToPresentation(summarizedText, imageParts);

                        var outputFile = Path.Combine(_environment.WebRootPath, "outputs", id + ".pptx");
                        presentation.Save(new FileStream(outputFile, FileMode.Create));
                        presentation.Close();

                        var downloadFileName = IO.Path.GetFileNameWithoutExtension(Upload.FileName) + ".pptx";
                        return PhysicalFile(outputFile, "application/vnd.openxmlformats-officedocument.presentationml.presentation", downloadFileName);
                    }
                }
            }
        }
    }
}
