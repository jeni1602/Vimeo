using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Vimeo2.Models;
using VimeoDotNet;
using VimeoDotNet.Models;
using VimeoDotNet.Net;
using Microsoft.AspNetCore.Http;

namespace Vimeo2.Controllers
{
    public class HomeController : Controller
    {
        string _accesstoken = "2d0d723a915716c7ea5293d7b5b897d6";

        public async Task<IActionResult> Index()
        {
            try
            {
                VimeoClient vimeoClient = new VimeoClient(_accesstoken);
                var authcheck = await vimeoClient.GetAccountInformationAsync();
                // You can use authcheck variable here if needed
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Debug.WriteLine(ex.Message);
            }
            return View();
        }

        public async Task<IActionResult> Upload(IFormFile file)
        {
            string uploadstatus = "";
            try
            {
                if (file != null)
                {
                    VimeoClient vimeoClient = new VimeoClient(_accesstoken);
                    var authcheck = await vimeoClient.GetAccountInformationAsync();

                    if (authcheck.Name != null)
                    {
                        BinaryContent binarycontent = new BinaryContent(file.OpenReadStream(), file.ContentType);

                        int chunksize = 0;
                        int contentlength = (int)file.Length;
                        int temp1 = contentlength / 1024;
                        if (temp1 > 1)
                        {
                            chunksize = temp1 / 1024;
                            chunksize = chunksize * 1048576;
                        }
                        else
                        {
                            chunksize = 1048576;
                        }

                        IUploadRequest uploadRequest = await vimeoClient.UploadEntireFileAsync(binarycontent, chunksize, null);
                        uploadstatus = string.Concat("File uploaded: ", "https://vimeo.com/", uploadRequest.ClipId.Value.ToString(), "/none");
                    }
                }
            }
            catch (Exception er)
            {
                uploadstatus = "Not uploaded: " + er.Message;
                if (er.InnerException != null)
                {
                    uploadstatus += " " + er.InnerException.Message;
                }
            }

            ViewBag.UploadStatus = uploadstatus;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
