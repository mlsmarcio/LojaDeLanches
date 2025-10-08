namespace LanchesMac.Service
{
    public interface ISeedUserRoleInitial
    {
        void SeedRoles(); // Para implementar a criação dos perfís
        void SeedUsers(); // Para criar os usuário e atribuir o usuário aos perfís
    }
}
