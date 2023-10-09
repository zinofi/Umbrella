using Umbrella.Utilities.Extensions;

namespace Umbrella.WebUtilities.Security;

public class PermissionsPolicyBuilder
{
	private readonly Dictionary<string, HashSet<string>> _permissionsDictionary = new(StringComparer.OrdinalIgnoreCase);

	public PermissionsPolicyBuilder()
	{
		_ = Accelerometer();
		_ = AmbientLightSensor();
		_ = Autoplay();
		_ = Battery();
		_ = Camera();
		_ = ClipboardRead();
		_ = ClipboardWrite();
		_ = CrossOriginIsolated();
		_ = DisplayCapture();
		_ = DocumentDomain();
		_ = EncryptedMedia();
		_ = ExecutionWhileNotRendered();
		_ = ExecutionWhileOutOfViewport();
		_ = Fullscreen();
		_ = Gamepad();
		_ = Geolocation();
		_ = Gyroscope();
		_ = KeyboardMap();
		_ = Magnetometer();
		_ = Microphone();
		_ = Midi();
		_ = NavigationOverride();
		_ = Payment();
		_ = PictureInPicture();
		_ = PublickeyCredentialsGet();
		_ = ScreenWakeLock();
		_ = SpeakerSelection();
		_ = SyncXhr();
		_ = Usb();
		_ = WebShare();
		_ = XrSpatialTracking();
	}

	public PermissionsPolicyBuilder Accelerometer(params string[] values) => AddNamedPolicy("accelerometer", values);
	public PermissionsPolicyBuilder AmbientLightSensor(params string[] values) => AddNamedPolicy("ambient-light-sensor", values);
	public PermissionsPolicyBuilder Autoplay(params string[] values) => AddNamedPolicy("autoplay", values);
	public PermissionsPolicyBuilder Battery(params string[] values) => AddNamedPolicy("battery", values);
	public PermissionsPolicyBuilder Camera(params string[] values) => AddNamedPolicy("camera", values);
	public PermissionsPolicyBuilder ClipboardRead(params string[] values) => AddNamedPolicy("clipboard-read", values);
	public PermissionsPolicyBuilder ClipboardWrite(params string[] values) => AddNamedPolicy("clipboard-write", values);
	public PermissionsPolicyBuilder CrossOriginIsolated(params string[] values) => AddNamedPolicy("cross-origin-isolated", values);
	public PermissionsPolicyBuilder DisplayCapture(params string[] values) => AddNamedPolicy("display-capture", values);
	public PermissionsPolicyBuilder DocumentDomain(params string[] values) => AddNamedPolicy("document-domain", values);
	public PermissionsPolicyBuilder EncryptedMedia(params string[] values) => AddNamedPolicy("encrypted-media", values);
	public PermissionsPolicyBuilder ExecutionWhileNotRendered(params string[] values) => AddNamedPolicy("execution-while-not-rendered", values);
	public PermissionsPolicyBuilder ExecutionWhileOutOfViewport(params string[] values) => AddNamedPolicy("execution-while-out-of-viewport", values);
	public PermissionsPolicyBuilder Fullscreen(params string[] values) => AddNamedPolicy("fullscreen", values);
	public PermissionsPolicyBuilder Gamepad(params string[] values) => AddNamedPolicy("gamepad", values);
	public PermissionsPolicyBuilder Geolocation(params string[] values) => AddNamedPolicy("geolocation", values);
	public PermissionsPolicyBuilder Gyroscope(params string[] values) => AddNamedPolicy("gyroscope", values);
	public PermissionsPolicyBuilder KeyboardMap(params string[] values) => AddNamedPolicy("keyboard-map", values);
	public PermissionsPolicyBuilder Magnetometer(params string[] values) => AddNamedPolicy("magnetometer", values);
	public PermissionsPolicyBuilder Microphone(params string[] values) => AddNamedPolicy("microphone", values);
	public PermissionsPolicyBuilder Midi(params string[] values) => AddNamedPolicy("midi", values);
	public PermissionsPolicyBuilder NavigationOverride(params string[] values) => AddNamedPolicy("navigation-override", values);
	public PermissionsPolicyBuilder Payment(params string[] values) => AddNamedPolicy("payment", values);
	public PermissionsPolicyBuilder PictureInPicture(params string[] values) => AddNamedPolicy("picture-in-picture", values);
	public PermissionsPolicyBuilder PublickeyCredentialsGet(params string[] values) => AddNamedPolicy("publickey-credentials-get", values);
	public PermissionsPolicyBuilder ScreenWakeLock(params string[] values) => AddNamedPolicy("screen-wake-lock", values);
	public PermissionsPolicyBuilder SpeakerSelection(params string[] values) => AddNamedPolicy("speaker-selection", values);
	public PermissionsPolicyBuilder SyncXhr(params string[] values) => AddNamedPolicy("sync-xhr", values);
	public PermissionsPolicyBuilder Usb(params string[] values) => AddNamedPolicy("usb", values);
	public PermissionsPolicyBuilder WebShare(params string[] values) => AddNamedPolicy("web-share", values);
	public PermissionsPolicyBuilder XrSpatialTracking(params string[] values) => AddNamedPolicy("xr-spatial-tracking", values);

	public PermissionsPolicyBuilder AddNamedPolicy(string name, params string[] values)
	{
		_permissionsDictionary[name.TrimToLowerInvariant()] = new HashSet<string>(values.Select(x => x.TrimToLowerInvariant()), StringComparer.Ordinal);

		return this;
	}

	/// <inheritdoc/>
	public override string ToString() => ToPermissionsPolicyString();

	public string ToPermissionsPolicyString() => string.Join(",", _permissionsDictionary.OrderBy(x => x.Key).Select(x =>
	{
		string values = x.Value.Count is 0 ? "" : string.Join(" ", x.Value.Select(x => x switch
		{
			not "none" and not "self" => $"\"{x}\"",
			_ => x
		}));

		return $"{x.Key}=({values})";
	}));

	public string ToFeaturePolicyString() => string.Join(";", _permissionsDictionary.OrderBy(x => x.Key).Select(x =>
	{
		string values = x.Value.Count is 0 ? "'none'" : string.Join(" ", x.Value.Select(x => x switch
		{
			"none" or "self" => $"'{x}'",
			_ => x
		}));

		return $"{x.Key} {values}";
	}));
}