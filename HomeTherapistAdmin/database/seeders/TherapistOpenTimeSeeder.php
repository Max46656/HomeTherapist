<?php

namespace Database\Seeders;

use App\Models\Calendar;
use App\Models\TherapistOpenTime;
use App\Models\User;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class TherapistOpenTimeSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {


        $users = User::all()->pluck('staff_id')->toArray();
        $calendars = Calendar::whereTime('dt', '>=', '08:00:00')
            ->whereTime('dt', '<=', '20:00:00')
            ->get()
            ->pluck('dt')
            ->toArray();

        config(['seeding.users' => $users]);
        config(['seeding.calendars' => $calendars]);

        $totalRecords = 500000;
        $batchSize = 2500;
        $totalBatches = ceil($totalRecords / $batchSize);
        for ($i = 0; $i < $totalBatches; $i++) {
            $data = TherapistOpenTime::factory()->count($batchSize)->make()->toArray();
            TherapistOpenTime::insert($data);
            sleep(0.1);
        }
    }
}