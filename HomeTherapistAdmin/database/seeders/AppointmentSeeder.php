<?php

namespace Database\Seeders;

use App\Models\AppointmentDetail;
use Database\Factories\AppointmentFactory;
use Faker\Factory as FakerFactory;
use Illuminate\Database\Seeder;

class AppointmentSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        // $faker = resolve(Faker::class);
        $faker = FakerFactory::create('zh_TW');
        $services = config('seeding.services');

        $totalRecords = 10000;
        $batchSize = 1000;
        $totalBatches = ceil($totalRecords / $batchSize);

        for ($i = 0; $i < $totalBatches; $i++) {
            for ($j = 0; $j < $batchSize; $j++) {
                $appointment = AppointmentFactory::new ()->create();

                $service = $faker->randomElement($services);

                $orderDetailData = [
                    'appointment_id' => $appointment->id,
                    'service_id' => $service['id'],
                    'price' => $service['price'],
                    'note' => $faker->sentence,
                ];
                AppointmentDetail::create($orderDetailData);
            }

            sleep(0.1);
        }
    }
}
