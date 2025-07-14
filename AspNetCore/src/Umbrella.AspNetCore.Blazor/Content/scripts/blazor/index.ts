/* eslint-disable */
import { BrowserEventAggregator } from './browserEventAggregator';

export class UmbrellaBlazorInterop
{
	#browserEventAggregator: BrowserEventAggregator | null = null;

	scrollTimeout: number;
	blazorInteropUtility: any;
	boundScrollTopFunction: any;

	get browserEventAggregator()
	{
		if (this.#browserEventAggregator)
			return this.#browserEventAggregator;

		this.#browserEventAggregator = new BrowserEventAggregator();

		return this.#browserEventAggregator;
	}

	public setPageTitle(title: string): void
	{
		document.title = title;
	}

	public scrollTo(position: number | string, offset = 0): void
	{
		if (typeof position === "number")
		{
			let offsetPosition = position + offset;

			if (offsetPosition < 0)
				offsetPosition = 0;

			window.scrollTo(offsetPosition, 0);

			return;
		}

		if (typeof position === "string")
		{
			const target = document.querySelector(position) as HTMLElement;

			if (target)
			{
				let offsetPosition = target.offsetTop + offset;

				if (offsetPosition < 0)
					offsetPosition = 0;

				window.scrollTo(offsetPosition, 0);

				return;
			}
		}
	}

	public scrollToBottom(): void
	{
		const bottom = window.outerHeight + 300;
		window.scrollTo(0, bottom);
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