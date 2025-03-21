namespace mostlylucid.pagingtaghelper.Models;

public enum ViewType
{
    [Obsolete("Use 'TailwindAndDaisy' instead.", false)]
    TailwindANdDaisy =0,
    Custom =1,
    Plain =2,
    Bootstrap =3,
   // Just a spelling fix to ensure back compat.
    TailwindAndDaisy =0
}