using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

class Program
{
    static void Main()
    {
        string[] drives = Environment.GetLogicalDrives();

        foreach (string drive in drives)
        {
            Console.WriteLine("Checking drive: " + drive);

            try
            {
                DirectoryInfo driveInfo = new DirectoryInfo(drive);
                SearchDirectories(driveInfo);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied for the drive.");
            }
        }
    }

    static void SearchDirectories(DirectoryInfo directory)
    {
        try
        {
            DirectoryInfo[] subDirectories = directory.GetDirectories();

            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                if (!IsUserHomeDirectory(subDirectory))
                {
                    CheckAndPrintPermissions(subDirectory.FullName);
                    SearchDirectories(subDirectory);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Access denied for the directory
            // No need to print an error message
        }
        catch (DirectoryNotFoundException)
        {
            // Directory not found
            // No need to print an error message
        }
        catch (ArgumentException)
        {
            //  ArgumentException
        }
        catch (NotSupportedException)
        {
            // NotSupportedException
        }
        try
        {
            FileInfo[] files = directory.GetFiles();

            foreach (FileInfo file in files)
            {
                CheckAndPrintPermissions(file.FullName);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Access denied for the file
            // No need to print an error message
        }
        catch (FileNotFoundException)
        {
            // File not found
            // No need to print an error message
        }
        catch (ArgumentException)
        {
            // Ignore ArgumentException
        }
        catch (NotSupportedException)
        {
            // NotSupportedException
        }
    }

    static bool IsUserHomeDirectory(DirectoryInfo directory)
    {
        string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return string.Equals(directory.FullName, userHomeDirectory, StringComparison.OrdinalIgnoreCase);
    }

    static void CheckAndPrintPermissions(string path)
    {
        try
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DirectorySecurity directorySecurity = directory.GetAccessControl();
            AuthorizationRuleCollection accessRules = directorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if (IsWriteAccessGroup(rule))
                {
                    Console.WriteLine("Write access granted to: " + path);
                    Console.WriteLine("Access granted to: " + GetGroupName(rule));
                    break;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Access denied for the file/directory
        }
        catch (PathTooLongException)
        {
            // Ignore path too long exception
        }
        catch (InvalidOperationException)
        {
            // Ignore invalid operation exception
        }
        catch (ArgumentException)
        {
            // Ignore ArgumentException
        }
        catch (NotSupportedException)
        {
            // NotSupportedException
        }
    }

    static bool IsWriteAccessGroup(FileSystemAccessRule accessRule)
    {
        SecurityIdentifier usersGroup = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
        SecurityIdentifier authenticatedUsersGroup = new SecurityIdentifier("S-1-5-11"); // SID for "Authenticated Users"
        SecurityIdentifier interactiveGroup = new SecurityIdentifier(WellKnownSidType.InteractiveSid, null);
        SecurityIdentifier everyoneGroup = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

        return (accessRule.IdentityReference.Equals(usersGroup) ||
                accessRule.IdentityReference.Equals(authenticatedUsersGroup) ||
                accessRule.IdentityReference.Equals(interactiveGroup) ||
                accessRule.IdentityReference.Equals(everyoneGroup)) &&
               accessRule.FileSystemRights.HasFlag(FileSystemRights.Write);
    }

    static string GetGroupName(FileSystemAccessRule accessRule)
    {
        SecurityIdentifier usersGroup = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
        SecurityIdentifier authenticatedUsersGroup = new SecurityIdentifier("S-1-5-11"); // SID for "Authenticated Users"
        SecurityIdentifier interactiveGroup = new SecurityIdentifier(WellKnownSidType.InteractiveSid, null);
        SecurityIdentifier everyoneGroup = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

        if (accessRule.IdentityReference.Equals(usersGroup))
        {
            return "BUILTIN\\Users";
        }
        else if (accessRule.IdentityReference.Equals(authenticatedUsersGroup))
        {
            return "NT AUTHORITY\\Authenticated Users";
        }
        else if (accessRule.IdentityReference.Equals(interactiveGroup))
        {
            return "NT AUTHORITY\\INTERACTIVE";
        }
        else if (accessRule.IdentityReference.Equals(everyoneGroup))
        {
            return "EVERYONE";
        }
        else
        {
            return "Unknown Group";
        }
    }
}
