<?php
namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;
use Spatie\Permission\Models\Permission;
use Spatie\Permission\Models\Role;

class AdminSeeder extends Seeder
{
    public function run()
    {
        $adminPermission = Permission::firstOrCreate(['name' => 'manage_admin']);

        $adminRole = Role::firstOrCreate(['name' => 'admin']);
        $adminRole->syncPermissions(Permission::all());
        $adminRole->givePermissionTo($adminPermission);

        $adminUser = User::create([
            'id' => Str::uuid(),
            'username' => $username = 'Admin',
            'normalized_username' => Str::lower($username),
            'email' => $email = 'admin@admin.com',
            'normalized_email' => Str::lower($email),
            'email_confirmed' => true,
            'password_hash' => Hash::make('123456'),
            'password' => Hash::make('123456'),
            'security_stamp' => Str::random(10),
            'concurrency_stamp' => Str::random(10),
            'phone_number' => '0912345678',
            'phone_number_confirmed' => true,
            'two_factor_enabled' => true,
            'lockout_end' => null,
            'lockout_enabled' => false,
            'access_failed_count' => 0,

            'staff_id' => 'A0001',
            'remember_token' => Str::random(10),
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        $adminUser->assignRole($adminRole);

    }
}
