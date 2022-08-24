namespace Application.Interfaces;

public interface IListDto<T>
{
    public int Count { get; set; }
    public ICollection<T> Items { get; set; }
}