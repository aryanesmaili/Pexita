using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;

namespace Pexita.Services.Interfaces
{
    public interface ITagsService
    {
        public bool AddTag(TagCreateVM product);
        public List<TagInfoVM> GetAllTags();
        public TagInfoVM GetTagByID(int id);
        public TagInfoVM UpdateTag(int id, TagCreateVM product);
        public bool DeleteTag(int id);
        public  List<TagInfoVM> TagsToVM(List<TagModel> tags);

    }
}
