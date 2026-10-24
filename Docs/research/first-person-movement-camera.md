# First-person movement and camera

*2026-10-24. Question: how should the player controller and camera be built in Unity 6000.6.6f1 with the Input System, so movement is comfortable and the camera does not jitter? Sources are mostly community threads (marked), plus Unity docs where they were reachable. Nothing here was measured in this project yet.*

## Sources
- Unity Manual, [CharacterController.Move](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/CharacterController.Move.html) (primary): Move does **not** apply gravity (apply it yourself); the example calls Move once per frame in `Update` with `Time.deltaTime`; read `isGrounded` at the start of the frame.
- Unity forum threads on camera judder, [first-person camera judder with rigidbody characters](https://discussions.unity.com/t/first-person-camera-judder-with-rigidbody-based-characters/178134) and [FixedUpdate or LateUpdate for camera movement](https://discussions.unity.com/t/fixedupdate-or-lateupdate-for-camera-movement/756203) (community, various Unity versions): jitter usually comes from the target and the camera running on different loops (physics ticks vs frames). A CharacterController moved in `Update`, with the camera in the same loop, removes it. Moving the camera to FixedUpdate ties the view to the fixed step, which is bad at varying frame rates.
- Unity forum, [Mouse jittering with new InputSystem](https://discussions.unity.com/t/mouse-jittering-with-new-inputsystem/856488) (community): for look, multiply the Input System mouse delta by a **constant** sensitivity and **do not** multiply by `Time.deltaTime`. The official Input System page for Mouse did not document delta scaling, so verify by testing at different frame rates.
- Madeline Miller, [Motion Sickness Accessibility in Video Games](https://madelinemiller.dev/blog/motion-sickness-accessibility/) (a developer article): offer an FOV slider, a view-bob toggle, adjustable or removable camera shake, a motion-blur toggle, and frame-rate options, and make them available before gameplay starts. It gives no default values and says nothing on acceleration curves or vignettes.
- Berry Bury Berry patch notes (our own research, [04-berry-bury-berry.md](../../../ClickOne/Docs/competitive/04-berry-bury-berry.md)): the first-person game shipped with "slidy" turning and extreme camera jitter on high-refresh monitors, and spent two weeks fixing movement, FOV, sprint and camera settings. The lesson we take: movement and camera are the biggest early risk.

## Decisions for the first slice
1. **CharacterController, moved in `Update`**, with gravity applied by us, and the camera a child of the player in the same loop. No Rigidbody character. (Source: Unity manual example plus the judder threads.)
2. **Look uses `Look` (mouse delta) times a constant sensitivity, no `Time.deltaTime`.** Pitch is clamped to about +/-85 degrees. (Source: community; test at two frame rates.)
3. **Movement is snappy, not slidy:** velocity approaches the target quickly (a short acceleration time), and stops quickly. The numbers (about 0.08 s up, 0.06 s down) are **a guess** to tune by feel.
4. **Comfort settings exist from day one, as serialized fields now and a settings screen later:** **horizontal** FOV (default **100, a guess**; the forum range of 70 to 110 is horizontal, as players usually mean it), mouse sensitivity, invert Y, head bob **off by default** (not built yet), always-sprint toggle. No camera shake or motion blur.
   - **Unit trap (primary source):** [`Camera.fieldOfView` is the *vertical* FOV](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Camera-fieldOfView.html); horizontal depends on the aspect ratio. So the setting is stored as horizontal and converted with `Camera.HorizontalToVerticalFieldOfView(hfov, aspect)`, re-applied when the aspect changes (ultrawide, window resize). B3 reportedly had a broken shop screen at ultrawide, so test aspect changes.
5. **Camera takeovers (the "pull" to the console) are not part of this slice.** When built, use a hard cut or short fade, never a swinging camera move (owner constraint and the B3 lesson).
6. **Game rules and math live in plain C# in their own assembly with Edit Mode tests** (kit rule 17): velocity approach and pitch clamp.
7. **Use the template's `InputSystem_Actions` asset as it is** (Player/Move, Look, Sprint). Input bindings are protected, so we read the asset and do not edit it.

## Not yet verified
- Feel (acceleration times, FOV default, sensitivity) needs a human to play it.
- Frame-rate independence of look needs a test at two frame rates.
- Behavior with a controller is untested.
