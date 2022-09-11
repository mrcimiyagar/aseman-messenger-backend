using System.Collections.Generic;
using HostVersion.DbContexts;
using HostVersion.Middles;
using Microsoft.EntityFrameworkCore;
using Version = HostVersion.Entities.Version;

namespace HostVersion.Utils
{
    public static class VersionHandler
    {
        public static void HandleVersionsUpdates(List<Version> versions)
        {
            /*if (versions == null) return;
            using (var dbContext = new DatabaseContext())
            {
                var tx = dbContext.Database.BeginTransaction();
                foreach (var version in versions)
                {
                    var versionIdParts = version.VersionId.Split("_");
                    dbContext.Database.ExecuteSqlCommand(
                        "insert into \"Versions\" (\"VersionId\", \"Number\") " +
                        $"values ('{versionIdParts[0]}_{versionIdParts[1]}', {version.Number}) " +
                        $"on conflict (\"VersionId\") do update set \"Number\" = case " +
                        $"when \"Versions\".\"Number\" > {version.Number} then \"Versions\".\"Number\" " +
                        $"else {version.Number} end;");
                }

                tx.Commit();
            }*/
        }

        public static void HandleVersionsFetchings(Packet packet)
        {
            /*using (var dbContext = new DatabaseContext())
            {
                var versions = new List<Version>();

                if (packet.Complex != null)
                {
                    var v = dbContext.Versions.Find("Complex_" + packet.Complex.ComplexId);
                    if (v != null)
                    {
                        packet.Complex.Version = v.Number;

                        versions.Add(v);
                    }
                }

                if (packet.Complexes != null && packet.Complexes.Count > 0)
                {
                    foreach (var complex in packet.Complexes)
                    {
                        var v = dbContext.Versions.Find("Complex_" + complex.ComplexId);
                        if (v != null)
                        {
                            complex.Version = v.Number;

                            versions.Add(v);
                        }
                    }
                }

                if (packet.BaseRoom != null)
                {
                    var v = dbContext.Versions.Find("Room_" + packet.BaseRoom.RoomId);
                    if (v != null)
                    {
                        packet.BaseRoom.Version = v.Number;

                        versions.Add(v);
                    }
                }

                if (packet.BaseRooms != null && packet.BaseRooms.Count > 0)
                {
                    foreach (var room in packet.BaseRooms)
                    {
                        var v = dbContext.Versions.Find("Room_" + room.RoomId);
                        if (v != null)
                        {
                            room.Version = v.Number;

                            versions.Add(v);
                        }
                    }
                }

                if (packet.User != null)
                {
                    var v = dbContext.Versions.Find("BaseUser_" + packet.User.BaseUserId);
                    if (v != null)
                    {
                        packet.User.Version = v.Number;

                        versions.Add(v);
                    }
                }

                if (packet.Users != null && packet.Users.Count > 0)
                {
                    foreach (var user in packet.Users)
                    {
                        var v = dbContext.Versions.Find("BaseUser_" + user.BaseUserId);
                        if (v != null)
                        {
                            user.Version = v.Number;

                            versions.Add(v);
                        }
                    }
                }

                if (packet.BaseUser != null)
                {
                    var v = dbContext.Versions.Find("BaseUser_" + packet.BaseUser.BaseUserId);
                    if (v != null)
                    {
                        packet.BaseUser.Version = v.Number;

                        versions.Add(v);
                    }
                }

                if (packet.Membership != null)
                {
                    var v = dbContext.Versions.Find("Membership_" + packet.Membership.MembershipId);
                    if (v != null)
                    {
                        packet.Membership.Version = v.Number;

                        versions.Add(v);
                    }
                }

                if (packet.Memberships != null && packet.Memberships.Count > 0)
                {
                    foreach (var membership in packet.Memberships)
                    {
                        var v = dbContext.Versions.Find("Membership_" + membership.MembershipId);
                        if (v != null)
                        {
                            membership.Version = v.Number;

                            versions.Add(v);
                        }
                    }
                }

                if (packet.MemberAccess != null)
                {
                    var v = dbContext.Versions.Find("MemberAccess_" + packet.MemberAccess.MemberAccessId);
                    if (v != null)
                    {
                        packet.MemberAccess.Version = v.Number;

                        versions.Add(v);
                    }
                }

                if (packet.MemberAccesses != null && packet.MemberAccesses.Count > 0)
                {
                    foreach (var memberAccess in packet.MemberAccesses)
                    {
                        var v = dbContext.Versions.Find("MemberAccess_" + memberAccess.MemberAccessId);
                        if (v != null)
                        {
                            memberAccess.Version = v.Number;

                            versions.Add(v);
                        }
                    }
                }

                packet.Versions = versions;
            }*/
        }
    }
}