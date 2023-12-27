using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangy_Models;

namespace Tangy_Busines.Repository.IRepository
{
    public interface ICategoryRepository
    {
        public CategoryDto Create(CategoryDto objDTO);
        public CategoryDto Update(CategoryDto objDTO);
        public int Delete(int id);
        public CategoryDto Get(int id);

        public IEnumerable<CategoryDto> GetAll();
    }
}
