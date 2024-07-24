using Pexita.Data;
using Pexita.Data.Entities.Tags;
using Pexita.Services.Interfaces;

namespace Pexita.Services
{
    public class TagsService : ITagsService
    {
        private readonly AppDBContext _Context;
        public TagsService(AppDBContext Context)
        {
            _Context = Context;
        }
        public bool AddTag(TagCreateVM product)
        {
            throw new NotImplementedException();
        }

        public bool DeleteTag(int id, string requestingUsername)
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

        public TagInfoVM UpdateTag(int id, TagCreateVM product, string requestingUsername)
        {
            throw new NotImplementedException();
        }
        public List<TagInfoVM> TagsToVM(List<TagModel> tags)
        {
            return tags.Select(x => new TagInfoVM { ID = x.ID, Title = x.Title, TimesUsed = x.TimesUsed }).ToList();
        }

        public bool IsTag(int id)
        {
            return _Context.Tags.FirstOrDefault(t => t.ID == id) != null;
        }

        public bool IsTag(string TagName)
        {
            if (_Context.Tags.FirstOrDefault(t => t.Title == TagName) == null)
            {
                _Context.Tags.Add(new TagModel() { Title = TagName });
            }
            return true;
        }
    }
}
