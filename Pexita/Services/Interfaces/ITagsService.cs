using Pexita.Data.Entities.Tags;

namespace Pexita.Services.Interfaces
{
    public interface ITagsService
    {
        public bool AddTag(TagCreateDTO product);
        public List<TagInfoDTO> GetAllTags();
        public TagInfoDTO GetTagByID(int id);
        public TagInfoDTO UpdateTag(int id, TagCreateDTO product, string requestingUsername);
        public bool DeleteTag(int id, string requestingUsername);
        public List<TagInfoDTO> TagsToDTO(List<TagModel> tags);
        public bool IsTag(int id);
        public bool IsTag(string TagName);

    }
}
