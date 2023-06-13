<?php

namespace App\Http\Controllers\Admin;

use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;
use Backpack\CRUD\app\Library\Widget;

/**
 * Class AppointmentCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class AppointmentCrudController extends CrudController
{
    use \Backpack\CRUD\app\Http\Controllers\Operations\ListOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\CreateOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\UpdateOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\DeleteOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\ShowOperation;

    /**
     * Configure the CrudPanel object. Apply settings to all operations.
     *
     * @return void
     */
    public function setup()
    {
        CRUD::setModel(\App\Models\Appointment::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/appointment');
        CRUD::setEntityNameStrings('appointment', 'appointments');
        $this->crud->denyAccess(['create', 'delete']);

    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {
        $completeAppointmentCount = \App\Models\Appointment::
            where('is_complete', true)->count();

        Widget::add()->to('before_content')->type('div')->class('row')->content([

            Widget::make()
                ->type('progress')
                ->class('card border-0 text-white bg-primary')
                ->progressClass('progress-bar')
                ->value($completeAppointmentCount)
                ->description('已完成預約')
                ->progress(100 * (int) $completeAppointmentCount / 1000)
                ->hint(1000 - $completeAppointmentCount % 1000 . '個預約到達下一個里程碑。'),
        ]);

        // CRUD::column('user_id');
        CRUD::column('user_id')
            ->type('relationship')
            ->attribute('user.staff_id')
            ->label('User')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    $user = \App\Models\User::where('staff_id', $entry->user_id)->first();
                    if ($user) {
                        return backpack_url('user/' . $user->id . '/show');
                    }
                    return backpack_url('user/');
                },
                'target' => '_blank',
            ])
            ->value(function ($entry) {
                $user = \App\Models\User::where('staff_id', $entry->user_id)->first();
                return $user->username ?? '-';
            });
        CRUD::column('start_dt')
            ->type('relationship')
            ->attribute('calendar.Dt')
            ->label('Calendar')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    // dump($entry->start_dt);
                    $date_time = \App\Models\Calendar::where('Dt', $entry->start_dt)->first();
                    if ($date_time) {
                        // dump($date_time);
                        return backpack_url('calendar/' . $date_time->id . '/show');
                    }
                    return backpack_url('user/');
                },
                'target' => '_blank',
            ])
            ->value(function ($entry) {
                return $entry->start_dt ?? '-';
            });
        CRUD::column('customer_ID');
        CRUD::column('customer_phone');
        CRUD::column('customer_address')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    $googleMapsLink = "https://www.google.com/maps/search/?api=1&query=" . urlencode($entry->customer_address);
                    return $googleMapsLink;
                },
                'target' => '_blank',
            ]);
        CRUD::column('is_complete');
        CRUD::column('latitude');
        CRUD::column('longitude');
        CRUD::column('created_at');
        CRUD::column('updated_at');

        /**
         * Columns can be defined using the fluent syntax or array syntax:
         * - CRUD::column('price')->type('number');
         * - CRUD::addColumn(['name' => 'price', 'type' => 'number']);
         */
    }

    /**
     * Define what happens when the Create operation is loaded.
     *
     * @see https://backpackforlaravel.com/docs/crud-operation-create
     * @return void
     */
    protected function setupCreateOperation()
    {

        // CRUD::setValidation(AppointmentRequest::class);
        CRUD::setValidation();
        CRUD::field('user_id');
        CRUD::field('start_dt');
        CRUD::field('customer_ID');
        CRUD::field('customer_phone');
        CRUD::field('customer_address');
        CRUD::field('latitude');
        CRUD::field('longitude');
        CRUD::field('is_complete');

        /**
         * Fields can be defined using the fluent syntax or array syntax:
         * - CRUD::field('price')->type('number');
         * - CRUD::addField(['name' => 'price', 'type' => 'number']));
         */
    }

    /**
     * Define what happens when the Update operation is loaded.
     *
     * @see https://backpackforlaravel.com/docs/crud-operation-update
     * @return void
     */
    protected function setupUpdateOperation()
    {
        $this->setupCreateOperation();
    }
}
