import "../styles/index.scss";
import { UmbrellaBlazorInterop } from "./blazor/index";

(() =>
{
	// eslint-disable-next-line
	// @ts-ignore
	window.UmbrellaBlazorInterop = new UmbrellaBlazorInterop();
})();