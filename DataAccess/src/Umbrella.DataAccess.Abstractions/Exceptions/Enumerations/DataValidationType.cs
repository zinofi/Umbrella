namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	// TODO: V3 - Why is this in here? Seems to be mostly being used with AspNetCore.
	// Maybe this was on the CostsBudgIT project somewhere? Need to check.
	public enum DataValidationType
	{
		Invalid = 400,
		Conflict = 409
	}
}