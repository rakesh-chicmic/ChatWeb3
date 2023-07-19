namespace ChatWeb3Frontend.Models
{
    public class PaginationCountList<T>
    {
        public int count { get; set; } = 0;
        public List<T> list { get; set; } = new List<T>();
        public PaginationCountList() { }
        public PaginationCountList(int count, List<T> list)
        {
            this.count = count;
            this.list = list;
        }
    }
}

// generic model for various response models that involves pagination