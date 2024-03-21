using ArticlProject.Core;
using ArticlProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArticlProject.Pages
{
    public class AllUsersModel : PageModel
    {
        public readonly int NoOfItem;
        private readonly IDataHelper<Core.Author> dataHelper;



        public AllUsersModel(
            IDataHelper<Core.Author> dataHelper
            )
        {
            this.dataHelper = dataHelper;
            NoOfItem = 2;
            ListOfAuthor = new List<Core.Author>();

        }

        public List<Core.Author> ListOfAuthor { get; set; }
        public void OnGet(string LoadState ,string search, int id)
        {

                        

            if (LoadState == null || LoadState == "All")
            {
                GetAllAuthors();
            }


            else if (LoadState == "Search")
            {
                SearchData(search);
            }
          

            else if (LoadState == "Next")
            {
                GetNextData(id);
            }

            else if (LoadState == "Prev")
            {
                GetPrevData(id-NoOfItem-1);
            }
        }



        // Get All Post

      

        private void GetAllAuthors()
        {
            ListOfAuthor = dataHelper.GetAllData().Take(NoOfItem).ToList();
        }


        private void SearchData(string SearchItem)
        {
            ListOfAuthor = dataHelper.Search(SearchItem);
        }

        private void GetNextData(int id)
        {
            ListOfAuthor = dataHelper.GetAllData().Where(x => x.Id > id).Take(NoOfItem).ToList();
        } 
        private void GetPrevData(int id)
        {
            ListOfAuthor = dataHelper.GetAllData().Where(x => x.Id > id).Take(NoOfItem).ToList();
        }



    }
}
