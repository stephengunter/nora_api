using ApplicationCore.Models;

namespace ApplicationCore.Exceptions;

public class CreateUserException : Exception
{
	public CreateUserException(User user, string msg = "") : base($"Create User Failed. UserName: {user.UserName} , {msg}")
	{

	}
}

public class UpdateUserException : Exception
{
	public UpdateUserException(User user, string msg = "") : base($"Update User Failed. UserName: {user.UserName} , {msg}")
	{

	}
}

public class UserNotFoundException : Exception
{
	public UserNotFoundException(string val, string key) : base($"UserNotFound. {key}: {val}")
	{

	}
}

public class UserSetPasswordException : Exception
{
	public UserSetPasswordException(User user, string msg = "") : base($"Set User Password Failed. UserName: {user.UserName} , {msg}")
	{
		
	}
}
