namespace ToDo_lists.Models
{
    public class SimilaritySearchResponseModel
    {
        public List<ResponseItem> responseItems { get; set; }
    }

    public class ResponseItem
    {
        public string to_do_list_name { get; set; }

        public int item_number { get; set; }

    }
}