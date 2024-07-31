using HelloDoc.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface IEncounterRepo
    {
        public Encountervm getEncounterFormData(int requestid, int status);

        public void setEncounterFormData( Encountervm updatedEncounterData,string aspId);
    }
}
