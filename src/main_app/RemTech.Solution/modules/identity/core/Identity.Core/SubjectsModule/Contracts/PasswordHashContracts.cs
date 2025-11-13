namespace Identity.Core.SubjectsModule.Contracts;

public delegate string HashPassword(string inputPassword);
public delegate bool VerifyPassword(string hashed, string inputPassword);