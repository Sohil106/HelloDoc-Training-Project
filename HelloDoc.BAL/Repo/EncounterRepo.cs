using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;

namespace HelloDoc.BAL.Repo
{
    public class EncounterRepo : IEncounterRepo
    {
        private readonly ApplicationDbContext _db;

        public EncounterRepo(ApplicationDbContext db)
        {
            _db = db;

        }

        /// <summary>
        /// Fetch EncounterTable Records On EncounterForm Page
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Encountervm getEncounterFormData(int requestid, int status)
        {
            var encounterData = _db.EncounterForms.FirstOrDefault(i => i.RequestId == requestid);
            var patientData = _db.RequestClients.FirstOrDefault(i => i.RequestId == requestid);
            var BirthDay = Convert.ToInt32(patientData.IntDate);
            var BirthMonth = Convert.ToInt32(patientData.StrMonth);
            var BirthYear = Convert.ToInt32(patientData.IntYear);

            if (encounterData != null)
            {
                Encountervm encountervm = new Encountervm()
                {
                    RequestId = requestid,
                    StatusForName = status,
                    FirstName = patientData.FirstName,
                    LastName = patientData.LastName,
                    Location = patientData.Street + " " + patientData.City + " " + patientData.State,
                    BirthDate = new DateTime(BirthYear, BirthMonth, BirthDay),
                    //ServiceDate
                    PhoneNumber = patientData.PhoneNumber,
                    Email = patientData.Email,
                    HistoryOfPresentIllness = encounterData.HistoryOfPresentIllnessOrInjury,
                    MedicalHistory = encounterData.MedicalHistory,
                    Medications = encounterData.Medications,
                    Allergies = encounterData.Allergies,
                    Temperature = encounterData.Temp,
                    HR = encounterData.Hr,
                    RR = encounterData.Rr,
                    BloodPressureSystolic = encounterData.BloodPressureSystolic,
                    BloodPressureDiastolic = encounterData.BloodPressureDiastolic,
                    O2 = encounterData.O2,
                    Pain = encounterData.Pain,
                    HEENT = encounterData.Heent,
                    CV = encounterData.Cv,
                    Chest = encounterData.Chest,
                    ABD = encounterData.Abd,
                    Extr = encounterData.Extremeties,
                    Skin = encounterData.Skin,
                    Neuro = encounterData.Neuro,
                    Other = encounterData.Other,
                    Diagnosis = encounterData.Diagnosis,
                    TreatmentPlan = encounterData.TreatmentPlan,
                    MedicationDispensed = encounterData.MedicationsDispensed,
                    Procedures = encounterData.Procedures,
                    FollowUp = encounterData.FollowUp,
                };
                return encountervm;
            }
            else
            {
                Encountervm encountervm = new Encountervm()
                {
                    RequestId = requestid,
                    StatusForName = status,
                    FirstName = patientData.FirstName,
                    LastName = patientData.LastName,
                    Location = patientData.Street + " " + patientData.City + " " + patientData.State,
                    BirthDate = new DateTime(BirthYear, BirthMonth, BirthDay),
                    //ServiceDate
                    PhoneNumber = patientData.PhoneNumber,
                    Email = patientData.Email,
                };
                return encountervm;
            }

        }

        /// <summary>
        /// Update Records From EncounterFrom Page
        /// </summary>
        /// <param name="updatedEncounterData"></param>
        /// <param name="aspId"></param>
        public void setEncounterFormData(Encountervm updatedEncounterData, string aspId)
        {
            var encounterData = _db.EncounterForms.FirstOrDefault(x => x.RequestId == updatedEncounterData.RequestId);
            var requstclientData = _db.RequestClients.FirstOrDefault(x => x.RequestId == updatedEncounterData.RequestId);
            var physician = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);
            var admin = _db.Admins.FirstOrDefault(x => x.AspNetUserId == aspId);

            if (encounterData != null)
            {
                var date = updatedEncounterData.BirthDate;
                requstclientData.IntDate = date.Day;
                requstclientData.StrMonth = date.Month.ToString();
                requstclientData.IntYear = date.Year;
                requstclientData.PhoneNumber = updatedEncounterData.PhoneNumber;
                encounterData.HistoryOfPresentIllnessOrInjury = updatedEncounterData.HistoryOfPresentIllness;
                encounterData.MedicalHistory = updatedEncounterData.MedicalHistory;
                encounterData.Allergies = updatedEncounterData.Allergies;
                encounterData.Temp = updatedEncounterData.Temperature;
                encounterData.Hr = updatedEncounterData.HR;
                encounterData.Rr = updatedEncounterData.RR;
                encounterData.BloodPressureDiastolic = updatedEncounterData.BloodPressureDiastolic;
                encounterData.BloodPressureSystolic = updatedEncounterData.BloodPressureSystolic;
                encounterData.O2 = updatedEncounterData.O2;
                encounterData.Pain = updatedEncounterData.Pain;
                encounterData.Heent = updatedEncounterData.HEENT;
                encounterData.Cv = updatedEncounterData.CV;
                encounterData.Chest = updatedEncounterData.Chest;
                encounterData.Abd = updatedEncounterData.ABD;
                encounterData.Extremeties = updatedEncounterData.Extr;
                encounterData.Skin = updatedEncounterData.Skin;
                encounterData.Neuro = updatedEncounterData.Neuro;
                encounterData.Other = updatedEncounterData.Other;
                encounterData.Diagnosis = updatedEncounterData.Diagnosis;
                encounterData.TreatmentPlan = updatedEncounterData.TreatmentPlan;
                encounterData.MedicationsDispensed = updatedEncounterData.MedicationDispensed;
                encounterData.Procedures = updatedEncounterData.Procedures;
                encounterData.FollowUp = updatedEncounterData.FollowUp;
                encounterData.PhysicianId = physician == null ? null : physician.PhysicianId;
                encounterData.AdminId = admin == null ? null : admin.AdminId;
                encounterData.Medications = updatedEncounterData?.Medications;
                _db.SaveChanges();
            }
            else
            {
                var encounterdata = new EncounterForm()
                {
                    RequestId = updatedEncounterData.RequestId,
                    HistoryOfPresentIllnessOrInjury = updatedEncounterData.HistoryOfPresentIllness,
                    MedicalHistory = updatedEncounterData.MedicalHistory,
                    Allergies = updatedEncounterData.Allergies,
                    Temp = updatedEncounterData.Temperature,
                    Hr = updatedEncounterData.HR,
                    Rr = updatedEncounterData.RR,
                    BloodPressureDiastolic = updatedEncounterData.BloodPressureDiastolic,
                    BloodPressureSystolic = updatedEncounterData.BloodPressureSystolic,
                    O2 = updatedEncounterData.O2,
                    Pain = updatedEncounterData.Pain,
                    Heent = updatedEncounterData.HEENT,
                    Cv = updatedEncounterData.CV,
                    Chest = updatedEncounterData.Chest,
                    Abd = updatedEncounterData.ABD,
                    Extremeties = updatedEncounterData.Extr,
                    Skin = updatedEncounterData.Skin,
                    Neuro = updatedEncounterData.Neuro,
                    Other = updatedEncounterData.Other,
                    Diagnosis = updatedEncounterData.Diagnosis,
                    TreatmentPlan = updatedEncounterData.TreatmentPlan,
                    MedicationsDispensed = updatedEncounterData.MedicationDispensed,
                    Procedures = updatedEncounterData.Procedures,
                    FollowUp = updatedEncounterData.FollowUp,
                    PhysicianId = physician == null ? null : physician.PhysicianId,
                    AdminId = admin == null ? null : admin.AdminId,
                    Medications = updatedEncounterData.Medications,

                };
                _db.EncounterForms.Add(encounterdata);
                _db.SaveChanges();
            }
        }
    }
}
