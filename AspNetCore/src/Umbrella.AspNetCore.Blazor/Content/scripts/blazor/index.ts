/* eslint-disable */
import animateScrollTo from 'animated-scroll-to';

export class UmbrellaBlazorInterop
{
	scrollTimeout: number;
	blazorInteropUtility: any;
	boundScrollTopFunction: any;

	constructor()
	{
	}

	public setPageTitle(title: string): void
	{
		document.title = title;
	}

	public animateScrollToAsync(position: number | string, offset = 0): Promise<boolean>
	{
		if (typeof position === "number")
		{
			let offsetPosition = position + offset;

			if (offsetPosition < 0)
				offsetPosition = 0;

			return animateScrollTo(offsetPosition);
		}

		if (typeof position === "string")
		{
			const target = document.querySelector(position) as HTMLElement;

			if (target)
			{
				let offsetPosition = target.offsetTop + offset;

				if (offsetPosition < 0)
					offsetPosition = 0;

				return animateScrollTo(offsetPosition);
			}
		}

		return Promise.resolve(true);
	}

	public animateScrollToBottomAsync(): Promise<boolean>
	{
		const bottom = window.outerHeight + 300;

		return animateScrollTo(bottom);
	}

	public initializeWindowScrolledTopAsync(blazorInteropUtility: any, threshold: number)
	{
		this.blazorInteropUtility = blazorInteropUtility;

		this.boundScrollTopFunction = this.windowScrolledTopAsync.bind(this, threshold);

		window.addEventListener("scroll", this.boundScrollTopFunction);
	}

	public destroyWindowScrolledTopAsync()
	{
		window.removeEventListener("scroll", this.boundScrollTopFunction);
	}

	private async windowScrolledTopAsync(threshold: number)
	{
		// If there's a timer, cancel it
		if (this.scrollTimeout)
			window.clearTimeout(this.scrollTimeout);

		this.scrollTimeout = window.setTimeout(async () =>
		{
			if (window.scrollY < threshold)
				await this.blazorInteropUtility.invokeMethodAsync("OnWindowScrolledTopAsync");
		}, 100);
	}
}