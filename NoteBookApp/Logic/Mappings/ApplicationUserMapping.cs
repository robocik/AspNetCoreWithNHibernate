using NHibernate.Mapping.ByCode.Conformist;
using NoteBookApp.Logic.Models;

namespace NoteBookApp.Logic.Mappings
{
    public class ApplicationUserMapping : JoinedSubclassMapping<ApplicationUser>
    {
        public ApplicationUserMapping()
        {

        }
    }
}