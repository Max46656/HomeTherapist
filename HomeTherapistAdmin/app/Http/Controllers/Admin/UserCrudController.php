<?php

namespace App\Http\Controllers\Admin;

use App\Http\Requests\UserRequest;
use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;

/**
 * Class UserCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class UserCrudController extends CrudController
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
        CRUD::setModel(\App\Models\User::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/user');
        CRUD::setEntityNameStrings('user', 'users');
    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {
        CRUD::column('username')->type('text');
        CRUD::column('email');
        CRUD::column('email_confirmed')->type('boolean');
        CRUD::column('staff_id');
        CRUD::column('certificate_number');
        CRUD::column('address');
        CRUD::column('latitude');
        CRUD::column('longitude');
        CRUD::column('radius');
        CRUD::column('password_hash');
        CRUD::column('security_stamp');
        CRUD::column('phone_number');
        CRUD::column('phone_number_confirmed')->type('boolean');
        CRUD::column('two_factor_enabled')->type('boolean');
        CRUD::column('lockout_end');
        CRUD::column('lockout_enabled')->type('boolean');
        CRUD::column('access_failed_count');
        CRUD::column('created_at')->type('datetime');
        CRUD::column('updated_at')->type('datetime');

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
        CRUD::setValidation(UserRequest::class);

        CRUD::field('username');
        CRUD::field('normalized_username');
        CRUD::field('email');
        CRUD::field('normalized_email');
        CRUD::field('email_confirmed');
        CRUD::field('password_hash');
        CRUD::field('security_stamp');
        CRUD::field('concurrency_stamp');
        CRUD::field('phone_number');
        CRUD::field('phone_number_confirmed');
        CRUD::field('two_factor_enabled');
        CRUD::field('lockout_end');
        CRUD::field('lockout_enabled');
        CRUD::field('access_failed_count');
        CRUD::field('staff_id');
        CRUD::field('address');
        CRUD::field('latitude');
        CRUD::field('longitude');

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