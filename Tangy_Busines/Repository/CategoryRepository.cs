using AutoMapper;
using Tangy_Busines.Repository.IRepository;
using Tangy_DataAccess;
using Tangy_DataAccess.Data;
using Tangy_Models;

namespace Tangy_Busines.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(ApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public CategoryDto Create(CategoryDto objDTO)
        {

            var obj = _mapper.Map<CategoryDto, Category>(objDTO); 
            obj.DateCreated = DateTime.Now;

            var addedOBJ = _context.categories.Add(obj);
            _context.SaveChanges();

            return _mapper.Map<Category, CategoryDto>(addedOBJ.Entity);
        }

        public int Delete(int id)
        {
            var obj = _context.categories.FirstOrDefault(u => u.Id == id);

            if(obj != null)
            {
                _context.categories.Remove(obj);
                return _context.SaveChanges();
            }
            return 0;
        }

        public CategoryDto Get(int id)
        {
            var obj = _context.categories.FirstOrDefault(u => u.Id == id);
            if(obj != null)
            {
                return _mapper.Map<Category, CategoryDto>(obj);
            }
            return new CategoryDto();
        }

        public IEnumerable<CategoryDto> GetAll()
        {
            return _mapper.Map<IEnumerable<Category>,IEnumerable<CategoryDto>>(_context.categories);
        }

        public CategoryDto Update(CategoryDto objDTO)
        {
            var objFromDb = _context.categories.FirstOrDefault(u =>u.Id == objDTO.Id);
            if(objFromDb != null)
            {
                objFromDb.Name = objDTO.Name;
                _context.categories.Update(objFromDb);
                _context.SaveChanges();

                return _mapper.Map<Category,CategoryDto>(objFromDb);
            }
            return objDTO;
        }
    }
}
