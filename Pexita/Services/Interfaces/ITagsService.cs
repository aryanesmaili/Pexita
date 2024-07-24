using Pexita.Data.Entities.Tags;

namespace Pexita.Services.Interfaces
{
    public interface ITagsService
    {
        public bool AddTag(TagCreateVM product);
        public List<TagInfoVM> GetAllTags();
        public TagInfoVM GetTagByID(int id);
        public TagInfoVM UpdateTag(int id, TagCreateVM product, string requestingUsername);
        public bool DeleteTag(int id, string requestingUsername);
        public List<TagInfoVM> TagsToVM(List<TagModel> tags);
        public bool IsTag(int id);
        public bool IsTag(string TagName);

    }
}
