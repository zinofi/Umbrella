﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Exceptions
{
	public enum DataValidationType
	{
		Invalid = 400,
		Conflict = 409
	}
}