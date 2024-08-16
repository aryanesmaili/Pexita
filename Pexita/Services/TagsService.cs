using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Tags;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Services
{
    public class TagsService(AppDBContext Context, IMapper mapper) : ITagsService
    {
        private readonly AppDBContext _Context = Context;
        private readonly IMapper _mapper = mapper;

        public async Task AddTag(string tagTitle)
        {
            TagModel tag = new() { Title = tagTitle, TimesUsed = 0, Products = [] };
            _Context.Tags.Add(tag);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteTag(int id)
        {
            TagModel tag = await _Context.Tags.FindAsync(id) ?? throw new NotFoundException($"Tag {id} not found");
            _Context.Tags.Remove(tag);
            await _Context.SaveChangesAsync();
        }

        public async Task<List<TagInfoDTO>> GetAllTags()
        {
            return await _Context.Tags
                .AsNoTracking()
                .Select(x => _mapper.Map<TagInfoDTO>(x))
                .ToListAsync();
        }

        public async Task<TagInfoDTO> GetTagByID(int id)
        {
            return _mapper.Map<TagInfoDTO>(await _Context.Tags.FindAsync(id));
        }

        public List<TagInfoDTO> TagsToDTO(List<TagModel> tags)
        {
            return tags.Select(x => new TagInfoDTO { ID = x.ID, Title = x.Title, TimesUsed = x.TimesUsed }).ToList();
        }

        public bool IsTag(int id)
        {
            return _Context.Tags.FirstOrDefault(t => t.ID == id) != null;
        }

        public bool IsTag(string TagName)
        {
            if (_Context.Tags.FirstOrDefault(t => t.Title == TagName) == null)
            {
                _Context.Tags.Add(new TagModel() { Title = TagName, Products = [] });
            }
            return true;
        }
    }
}
