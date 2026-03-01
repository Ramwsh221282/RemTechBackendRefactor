using FluentMigrator;

namespace TimeZones.Infrastructure.Migrations
{
	[Migration(202501080018)]
	public sealed class ApplicationTimeZoneMigration : Migration
	{
		public override void Down()
		{
			throw new NotImplementedException();
		}

		public override void Up()
		{
			Execute.Sql(
				"""
				KeyPress event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39587, (349,203), root:(6019,1211),
    state 0x0, keycode 66 (keysym 0xffe5, Caps_Lock), same_screen YES,
    XLookupString gives 0 bytes: 
    XmbLookupString gives 0 bytes: 
    XFilterEvent returns: False

KeyPress event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39589, (349,203), root:(6019,1211),
    state 0x2, keycode 37 (keysym 0xffe3, Control_L), same_screen YES,
    XLookupString gives 0 bytes: 
    XmbLookupString gives 0 bytes: 
    XFilterEvent returns: False

KeyPress event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39589, (349,203), root:(6019,1211),
    state 0x6, keycode 64 (keysym 0xffe9, Alt_L), same_screen YES,
    XLookupString gives 0 bytes: 
    XmbLookupString gives 0 bytes: 
    XFilterEvent returns: False

KeyPress event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39589, (349,203), root:(6019,1211),
    state 0xe, keycode 133 (keysym 0xffeb, Super_L), same_screen YES,
    XLookupString gives 0 bytes: 
    XmbLookupString gives 0 bytes: 
    XFilterEvent returns: False

KeyPress event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39589, (349,203), root:(6019,1211),
    state 0x4e, keycode 105 (keysym 0xffe4, Control_R), same_screen YES,
    XLookupString gives 0 bytes: 
    XmbLookupString gives 0 bytes: 
    XFilterEvent returns: False

KeyPress event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39589, (349,203), root:(6019,1211),
    state 0x4e, keycode 62 (keysym 0xffe2, Shift_R), same_screen YES,
    XLookupString gives 0 bytes: 
    XmbLookupString gives 0 bytes: 
    XFilterEvent returns: False

KeyPress event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39589, (349,203), root:(6019,1211),
    state 0x4f, keycode 108 (keysym 0xffea, Alt_R), same_screen YES,
    XLookupString gives 0 bytes: 
    XmbLookupString gives 0 bytes: 
    XFilterEvent returns: False

KeyPress event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39589, (349,203), root:(6019,1211),
    state 0x4f, keycode 134 (keysym 0xffec, Super_R), same_screen YES,
    XLookupString gives 0 bytes: 
    XmbLookupString gives 0 bytes: 
    XFilterEvent returns: False

KeyRelease event, serial 39, synthetic NO, window 0x1600001,
    root 0x393, subw 0x0, time 39677, (349,203), root:(6019,1211),
    state 0x4f, keycode 66 (keysym 0xffe5, Caps_Lock), same_screen YES,
    XLookupString gives 0 bytes: 
    XFilterEvent returns: False

				"""
			);
		}
	}
}
