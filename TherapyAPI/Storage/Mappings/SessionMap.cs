using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class SessionMap : ClassMap<Session>
    {
        public SessionMap()
        {
            Table("sessions");
            Id(u => u.ID, "id");

            References(e => e.Problem, "id_problem");
            References(e => e.Specialist, "id_specialist");

            Map(u => u.Status, "session_status").CustomType<SessionStatus>();
            Map(u => u.Reward, "reward");
            Map(u => u.Date, "session_date");
            Map(u => u.IsSpecialistClose, "is_specialist_close").Not.Nullable();
            Map(u => u.IsClientClose, "is_client_close").Not.Nullable();
            Map(u => u.SpecialistCloseDate, "specialist_close_date");
            Map(u => u.ClientCloseDate, "client_close_date");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
