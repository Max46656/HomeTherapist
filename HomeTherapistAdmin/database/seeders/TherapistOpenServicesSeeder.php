<?php

namespace Database\Seeders;

use App\Models\TherapistOpenServices;
use Illuminate\Database\Seeder;

class TherapistOpenServicesSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {

        $totalRecords = 6000;
        $batchSize = 1000;
        $totalBatches = ceil($totalRecords / $batchSize);
        for ($i = 0; $i < $totalBatches; $i++) {
            TherapistOpenServices::factory()->count($batchSize)->create();
            sleep(0.1);
        }

    }
}