// DEFINITIONS FOR QUERY FILTERS
static public class ERROR_CODES
{
	// List of error codes
	// Error codes are int
	// API COMMUNICATION ERROR CODES
	public const int 	RESP_OK 				= 200;
	public const int 	RESP_BAD_CALL 			= 400;
	public const int 	RESP_BAD_PARAMS 		= 401;
	public const int 	RESP_BAD_HASH_KEY		= 402;
	public const int	RESP_NOT_FOUND			= 404;
	// INTERNET CONNECTION ERROR CODES
	public const int 	NO_INTERNET_CONNECTION 	= 500;
	public const int 	INTERNET_VERIFIED		= 510;

}

public class ErrorObject
{
	public int errorCode { get; private set; }
	public string errorDesc { get; private set; }

	public ErrorObject (int errorCode, string errorDesc )
	{
		this.errorCode = errorCode;
		this.errorDesc = errorDesc;
	}

	public ErrorObject( ErrorObject errorObj )
	{
		this.errorCode = errorObj.errorCode;
		this.errorDesc = errorObj.errorDesc;
	}
}

public class ErrorEvent : Deprecated.IEvent
{
	private ErrorObject _errorObj;
	
	public ErrorEvent( int errorCode, string errorDesc )
	{
		_errorObj = new ErrorObject( errorCode, errorDesc );
	}

    string Deprecated.IEvent.GetCallName()
	{
		return this._errorObj.errorCode.ToString();
	}

    string Deprecated.IEvent.GetName()
	{
		return this.GetType().ToString();
	}

    object Deprecated.IEvent.GetData()
	{
		return this._errorObj;
	}
}
