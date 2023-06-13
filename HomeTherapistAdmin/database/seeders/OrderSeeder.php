<?php

namespace Database\Seeders;

use App\Models\Appointment;
use App\Models\Feedback;
use App\Models\Order;
use App\Models\OrderDetail;
use Faker\Factory as FakerFactory;
use Illuminate\Database\Seeder;

class OrderSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        $completedAppointments = Appointment::where('is_complete', true)->get();

        $services = config('seeding.services');
        // $faker = resolve(Faker::class);
        $faker = FakerFactory::create('zh_TW');

        foreach ($completedAppointments as $appointment) {
            $orderData = [
                'user_id' => $appointment->user_id,
                'start_dt' => $appointment->start_dt,
                'customer_ID' => $appointment->customer_ID,
                'customer_phone' => $appointment->customer_phone,
                'customer_address' => $appointment->customer_address,
                'latitude' => $appointment->latitude,
                'longitude' => $appointment->longitude,
                'gender' => $appointment->gender,
                'age_group' => $appointment->age_group,
                'is_complete' => true,
                'created_at' => now(),
                'updated_at' => now(),
            ];

            $order = Order::create($orderData);
            $service = $services->random();

            $orderDetailData = [
                'order_id' => $order->id,
                'service_id' => $service['id'],
                'price' => $service['price'],
                'note' => $faker->sentence,
            ];

            OrderDetail::create($orderDetailData);

            $feedbackData = [
                'user_id' => $order->user_id,
                'order_id' => $order->id,
                'customer_id' => $order->customer_ID,
                'comments' => $faker->realText(100, 2),
                'rating' => $faker->numberBetween(1, 5),
                'created_at' => now(),
                'updated_at' => now(),
            ];

            Feedback::create($feedbackData);

            sleep(0.0001);
        }

    }
}
