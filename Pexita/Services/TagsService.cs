using Pexita.Data.Entities.Tags;
using Pexita.Services.Interfaces;

namespace Pexita.Services
{
    public class TagsService : ITagsService
    {
        public bool AddTag(TagCreateVM product)
        {
            throw new NotImplementedException();
        }

        public bool DeleteTag(int id)
        {
            throw new NotImplementedException();
        }

        public List<TagInfoVM> GetAllTags()
        {
            throw new NotImplementedException();
        }

        public TagInfoVM GetTagByID(int id)
        {
            throw new NotImplementedException();
        }

        public TagInfoVM UpdateTag(int id, TagCreateVM product)
        {
            throw new NotImplementedException();
        }
        public List<TagInfoVM> TagsToVM(List<TagModel> tags)
        {
           return tags.Select(x => new TagInfoVM { ID = x.ID, Title = x.Title, TimesUsed = x.TimesUsed }).ToList();
        }
    }
}
