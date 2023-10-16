export class BrowserEventAggregator
{
	#subscriptionMap = new Map<string, BrowserEventSubscription[]>();
	#eventHandlerMap = new Map<string, () => Promise<void>>();

	public addEventListener(id: string, eventName: string, dotNetObjectReference: DotNetObjectReference): void
	{
		let subscriptions = this.#subscriptionMap.get(eventName);

		if (!subscriptions)
		{
			subscriptions = new Array<BrowserEventSubscription>();
			this.#subscriptionMap.set(eventName, subscriptions);

			let eventHandler = this.#eventHandlerMap.get(eventName);

			if (!eventHandler)
			{
				eventHandler = this.notifyEventSubscribersAsync.bind(this, eventName);
				this.#eventHandlerMap.set(eventName, eventHandler);
			}

			window.addEventListener(eventName, eventHandler);
		}

		subscriptions.push(new BrowserEventSubscription(id, eventName, dotNetObjectReference));
	}

	public removeEventListener(id: string, eventName: string): void
	{
		const subscriptions = this.#subscriptionMap.get(eventName);

		if (!subscriptions)
			return;

		const subscription = subscriptions.find(x => x.id === id);

		if (subscription)
		{
			const updatedSubscriptions = subscriptions.filter(x => x !== subscription);

			if (updatedSubscriptions.length > 0)
			{
				this.#subscriptionMap.set(eventName, updatedSubscriptions);
			}
			else
			{
				this.#subscriptionMap.delete(eventName);

				const eventHandler = this.#eventHandlerMap.get(eventName);

				if (eventHandler)
					window.removeEventListener(eventName, eventHandler);

				this.#eventHandlerMap.delete(eventName);
			}
		}
	}

	private async notifyEventSubscribersAsync(eventName: string): Promise<void>
	{
		const subscriptions = this.#subscriptionMap.get(eventName);

		if (subscriptions)
		{
			for (let item of subscriptions)
			{
				await item.publishAsync();
			}
		}
	}
}

class BrowserEventSubscription
{
	constructor(public id: string, public eventName: string, private dotNetObjectReference: DotNetObjectReference)
	{
	}

	public async publishAsync(): Promise<void>
	{
		await this.dotNetObjectReference.invokeMethodAsync("PublishAsync", this.eventName);
	}
}

declare type DotNetObjectReference = {
	invokeMethodAsync: (methodName: string, ...args: any) => Promise<void>
}