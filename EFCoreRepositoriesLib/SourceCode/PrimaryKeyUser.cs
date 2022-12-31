using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreRepositoriesLib;

internal interface IPrimaryKeyUser
{
}

public class PrivatePrimaryKeyUser : IPrimaryKeyUser
{
    [AdaptMember(nameof(PublicPrimaryKeyUser.ID))]
    private protected int _id { get; set; }
}

public class ReadOnlyPrimaryKeyUser : PrivatePrimaryKeyUser
{
    public int ID { get => _id; private set => _id = value; }
}

public class PublicPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    public new int ID { get => _id; set => _id = value; }
}