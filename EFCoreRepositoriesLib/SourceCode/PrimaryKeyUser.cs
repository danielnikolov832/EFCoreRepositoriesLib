using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreRepositoriesLib;

public interface IPrivatePrimaryKeyUser
{
}

public interface IReadOnlyPrimaryKeyUser : IPrivatePrimaryKeyUser
{
    public int ID { get; }
}

public interface IPublicPrimaryKeyUser : IReadOnlyPrimaryKeyUser
{
    public new int ID { get; set; }
}

public class PrivatePrimaryKeyUser : IPrivatePrimaryKeyUser
{
    [AdaptMember(nameof(PublicPrimaryKeyUser.ID))]
    private protected int _id { get; set; }
}

public class ReadOnlyPrimaryKeyUser : PrivatePrimaryKeyUser, IReadOnlyPrimaryKeyUser
{
    public int ID { get => _id; private set => _id = value; }
}

public class PublicPrimaryKeyUser : ReadOnlyPrimaryKeyUser, IPublicPrimaryKeyUser
{
    public new int ID { get => _id; set => _id = value; }
}

public record class RecordPrivatePrimaryKeyUser : IPrivatePrimaryKeyUser
{
    [AdaptMember(nameof(PublicPrimaryKeyUser.ID))]
    private protected int _id { get; set; }
}

public record class RecordReadOnlyPrimaryKeyUser : RecordPrivatePrimaryKeyUser, IReadOnlyPrimaryKeyUser
{
    public int ID { get => _id; private set => _id = value; }
}

public record class RecordPublicPrimaryKeyUser : RecordReadOnlyPrimaryKeyUser, IPublicPrimaryKeyUser
{
    public new int ID { get => _id; set => _id = value; }
}