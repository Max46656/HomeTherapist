<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Seeder;
use Spatie\Permission\Models\Role;

class UserSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run()
    {
        $role = Role::firstOrCreate(['name' => 'User']);

        $totalRecords = 2000;
        $batchSize = 100;
        $totalBatches = ceil($totalRecords / $batchSize);
        for ($i = 0; $i < $totalBatches; $i++) {
            User::factory()->count($batchSize)->create()
                ->each(function ($user) use ($role) {
                    $user->assignRole($role);
                });
            sleep(0.05);
        }
    }
}
